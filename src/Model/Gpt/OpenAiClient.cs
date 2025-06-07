using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Model.Domain;
using OpenAI;
using OpenAI.Responses;

namespace Model.Gpt
{
    public class OpenAIClient(string apiKey) : IGptClient
    {
        private readonly OpenAIResponseClient _openAiResponseClient = new(
            "gpt-4.1",
            new ApiKeyCredential(apiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://api.openai.com/v1")
            });

        public async Task<INewsProcessorResult> ProcessNewsAsync(string newsUrl)
        {
            try
            {
                return await SafeProcessNewsAsync(newsUrl);
            }
            catch (Exception exception)
            {
                return NewsProcessorErrorResultFactory.Create(exception);
            }
        }

        private async Task<INewsProcessorResult> SafeProcessNewsAsync(string newsUrl)
        {
            var userInputText = GptPrompts.NewsToHypothesis.Replace("{{URL}}", newsUrl);

            OpenAIResponse openAiResponse = await _openAiResponseClient.CreateResponseAsync(
                userInputText: userInputText,
                new ResponseCreationOptions
                {
                    Tools = { ResponseTool.CreateWebSearchTool() },
                });

            var messageResponseItem = GetSingleMessageResponseItem(openAiResponse);
            var messageResponseText = GetMessageResponseItemContent(messageResponseItem);

            return AnalyzeMessageResponseText(messageResponseText);
        }

        private static INewsProcessorResult AnalyzeMessageResponseText(string messageResponseText)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            NewsProcessorSuccessResult? newsProcessorSuccessResult;
            try
            {
                newsProcessorSuccessResult = JsonSerializer.Deserialize<NewsProcessorSuccessResult>(
                    messageResponseText,
                    options);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to deserialize response")
                {
                    Data =
                    {
                        ["Response"] = messageResponseText
                    }
                };
            }

            if (newsProcessorSuccessResult is null)
            {
                throw new InvalidOperationException("Response after deserialization is null")
                {
                    Data =
                    {
                        ["Response"] = messageResponseText
                    }
                };
            }

            return newsProcessorSuccessResult;
        }

        private static string GetMessageResponseItemContent(MessageResponseItem messageResponseItem)
        {
            if (messageResponseItem == null)
                throw new ArgumentNullException(nameof(messageResponseItem), "MessageResponseItem is null");

            if (messageResponseItem.Content == null || !messageResponseItem.Content.Any())
                throw new InvalidOperationException("MessageResponseItem.Content is null or empty");

            var firstContent = messageResponseItem.Content.FirstOrDefault();
            if (firstContent == null || string.IsNullOrWhiteSpace(firstContent.Text))
                throw new InvalidOperationException("MessageResponseItem.Content does not contain valid text");

            return firstContent.Text;
        }

        private static MessageResponseItem GetSingleMessageResponseItem(OpenAIResponse response)
        {
            var messages = response.OutputItems
                .OfType<MessageResponseItem>()
                .ToList();

            return messages.Count switch
            {
                0 => throw new InvalidOperationException("No assistant message found in the response"),
                > 1 => throw new InvalidOperationException(
                    $"Expected exactly one assistant message, but found {messages.Count}"),
                _ => messages[0]
            };
        }
    }
}
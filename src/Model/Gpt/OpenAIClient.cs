using System.ClientModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Model.Domain;
using OpenAI;
using OpenAI.Responses;

namespace Model.Gpt;

public class OpenAIClient(IOptions<GptOptions> gptOptions) : IGptClient
{
    private readonly OpenAIResponseClient _openAiResponseClient = new(
        "gpt-4.1",
        new ApiKeyCredential(gptOptions.Value.ApiKey),
        new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.openai.com/v1")
        });

    public async Task<INewsProcessorResult> ProcessNewsAsync(string newsUrl, int sourceReliability)
    {
        try
        {
            return await ProcessNewsSafeAsync(newsUrl, sourceReliability);
        }
        catch (Exception exception)
        {
            return NewsProcessorErrorResultFactory.Create(newsUrl, exception);
        }
    }

    private async Task<INewsProcessorResult> ProcessNewsSafeAsync(string newsUrl, int sourceReliability)
    {
        var stopwatch = Stopwatch.StartNew();

        var userInputText = GptPrompts.NewsToHypothesis
            .Replace("{{ URL }}", newsUrl)
            .Replace("{{ SOURCE_RELIABILITY }}", sourceReliability.ToString());

        OpenAIResponse openAiResponse = await _openAiResponseClient.CreateResponseAsync(
            userInputText: userInputText,
            new ResponseCreationOptions
            {
                Tools = { ResponseTool.CreateWebSearchTool() },
            });

        var messageResponseItem = GetSingleMessageResponseItem(openAiResponse);
        var messageResponseText = GetMessageResponseItemContent(messageResponseItem);

        var newsAnalyze = AnalyzeMessageResponseText(messageResponseText);

        return new NewsProcessorSuccessResult(
            NewsUrl: newsUrl,
            NewsAnalyze: newsAnalyze,
            UsageTotalTokenCount: openAiResponse.Usage.TotalTokenCount,
            ProcessingDurationTime: stopwatch.Elapsed);
    }

    private static NewsAnalyze AnalyzeMessageResponseText(string messageResponseText)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        NewsAnalyze? newsAnalyze;
        try
        {
            newsAnalyze = JsonSerializer.Deserialize<NewsAnalyze>(
                messageResponseText,
                options);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("Failed to deserialize response", exception)
            {
                Data =
                {
                    ["Response"] = messageResponseText
                }
            };
        }

        if (newsAnalyze is null)
        {
            throw new InvalidOperationException("NewsAnalyze after deserialization is null")
            {
                Data =
                {
                    ["Response"] = messageResponseText
                }
            };
        }

        return newsAnalyze;
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
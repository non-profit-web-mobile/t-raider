using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using Model.Domain;

namespace Services
{
    public class GPTClient
    {
        private readonly string _systemPrompt;
        private readonly OpenAIClient _openAIClient;

        public GPTClient(string systemPrompt, string apiKey)
        {
            _systemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
            _openAIClient = new OpenAIClient(new OpenAIAuthentication(apiKey));
        }

        public async Task<NewsProcessingResult?> AnalyzeNewsAsync(string newsUrl)
        {
            if (string.IsNullOrWhiteSpace(newsUrl))
                throw new ArgumentException("News URL cannot be empty", nameof(newsUrl));

            try
            {
                var messages = new List<Message>
                {
                    new Message(Role.System, _systemPrompt + "\nИспользуй веб-поиск для анализа ссылки и содержимого страницы."),
                    new Message(Role.User, $"Проанализируй эту новость: {newsUrl}")
                };
                
                var chatRequest = new ChatRequest(
                    messages: messages,
                    model: "gpt-4-turbo",
                    tools: new List<Tool> {  },
                    toolChoice: "auto"
                );
                
                var response = await _openAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
                
                var content = response.FirstChoice.Message.Content;

                if (string.IsNullOrWhiteSpace(content))
                    return null;
                
                return JsonSerializer.Deserialize<NewsProcessingResult>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing news: {ex.Message}");
                throw;
            }
        }
    }
}
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Model.Domain;

namespace Model.Gpt
{
	public class BothubChatGptClient
	{
		private readonly HttpClient _httpClient;
		private readonly string _prompt;
		private const string ApiUrl = "https://bothub.chat/api/v2/openai/v1/chat/completions";

		public BothubChatGptClient(string apiKey, string prompt)
		{
			_prompt = prompt;
			_httpClient = new HttpClient { BaseAddress = new Uri(ApiUrl) };
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
		}

		public async Task<NewsProcessingResult?> ProcessNewsAsync(string newsUrl)
		{
			var promptWithUrl = _prompt.Replace("{{URL}}", newsUrl);
			var requestBody = new
			{
				model = "gpt-4o-search-preview",
				prompt = promptWithUrl,
				web_search_options = new
				{
				}
			};

			var response = await _httpClient.PostAsJsonAsync("", requestBody);
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadAsStringAsync();

			using var doc = JsonDocument.Parse(json);
			var content = doc.RootElement
				.GetProperty("choices")[0]
				.GetProperty("text")
				.GetString();

			if (string.IsNullOrWhiteSpace(content))
				return new NewsProcessingResult(null, new ErrorResult("Empty response from model"));
			try
			{
				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
				};
				var success = JsonSerializer.Deserialize<SuccessResult>(content, options);
				if (success != null)
					return new NewsProcessingResult(success, null);
				else
					return new NewsProcessingResult(null, new ErrorResult(content));
			}
			catch (JsonException ex)
			{
				return new NewsProcessingResult(null,
					new ErrorResult(content));
			}
		}
	}
}
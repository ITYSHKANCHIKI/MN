// File: backend/MoralNavigator.API/Services/ChatGptService.cs

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MoralNavigator.API.Services
{
    public record ChatMessageDto(string role, string content);

    public interface IChatGptService
    {
        Task<string> SendChatAsync(IEnumerable<ChatMessageDto> messages);
    }

    public class ChatGptService : IChatGptService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public ChatGptService(IConfiguration configuration, IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient();
            _apiKey = configuration["OpenAI:ApiKey"]!;
        }

        public async Task<string> SendChatAsync(IEnumerable<ChatMessageDto> messages)
        {
            var requestBody = new
            {
                model = "gpt-4o",
                messages = messages
            };

            var json = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _http.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API responded {(int)response.StatusCode}: {body}");
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);

            var choice = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return choice ?? string.Empty;
        }
    }
}

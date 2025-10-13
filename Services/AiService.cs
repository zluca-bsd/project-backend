using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using project_backend.Dtos.AiDtos;
using project_backend.Dtos.BookDtos;
using project_backend.Services.Interfaces;

namespace project_backend.Services
{
    public class AiService : IAiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AiSettings _aiSettings;
        private readonly IBooksService _booksService;

        public AiService(IHttpClientFactory httpClientFactory, IOptions<AiSettings> aiSettings, IBooksService booksService)
        {
            _httpClientFactory = httpClientFactory;
            _aiSettings = aiSettings.Value;
            _booksService = booksService;
        }

        public async Task<AiResponseDto> GetChatResponseAsync(AiRequestDto request)
        {
            var client = _httpClientFactory.CreateClient();
            var aiPayload = new
            {
                model = _aiSettings.Model,
                temperature = _aiSettings.Temperature,
                max_tokens = _aiSettings.MaxTokens,
                stream = _aiSettings.Stream,
                messages = new[]
                {
                    new { role = "system", content = "You are an library assistant that can access the database." },
                    new { role = "user", content = request.Prompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(aiPayload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_aiSettings.Endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"AI request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseString);
            var root = document.RootElement;

            // Parse AI message
            var message = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new AiResponseDto
            {
                Response = message
            };
        }
    }
}
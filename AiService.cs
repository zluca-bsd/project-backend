using System.Text;
using System.Text.Json;

namespace project_backend
{
    public class AiService
    {
        private readonly HttpClient _httpClient;
        private readonly AiSettings _aiSettings;

        public AiService(HttpClient httpClient, AiSettings aiSettings)
        {
            _httpClient = httpClient;
            _aiSettings = aiSettings;
        }

        public async Task<string> GetAiResponseAsync(string question)
        {
            var request = new
            {
                model = _aiSettings.Model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = question }
                },
                temperature = _aiSettings.Temperature,
                max_tokens = _aiSettings.MaxTokens,
                stream = _aiSettings.Stream
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_aiSettings.Endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"LM Studio error: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var reply = doc.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString();

            if (reply == null)
            {
                throw new Exception("Reply is null:");
            }

            return reply;
        }
    }
}
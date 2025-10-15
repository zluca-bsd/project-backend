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
        private readonly IBooksService _bookService;
        private readonly List<AiToolDefinition> _tools;

        public AiService(IHttpClientFactory httpClientFactory, IOptions<AiSettings> aiSettings, IBooksService booksService)
        {
            _httpClientFactory = httpClientFactory;
            _aiSettings = aiSettings.Value;
            _bookService = booksService;

            var json = File.ReadAllText("AiTools/tools.json");
            _tools = JsonSerializer.Deserialize<List<AiToolDefinition>>(json) ?? new List<AiToolDefinition>();
        }

        public async Task<AiResponse> AskAi(string userPrompt)
        {
            var systemPrompt = $"""
            You are a library assistant that answers user's request.
            You have access to tools.
            """;

            var aiMessage = await HandleChatWithToolsAsync(systemPrompt, userPrompt);
            return new AiResponse { Response = aiMessage };
        }

        private async Task<string> HandleChatWithToolsAsync(string systemPrompt, string userPrompt)
        {
            var httpClient = CreateHttpClient();

            var requestBody = new
            {
                model = _aiSettings.Model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                tools = _tools
            };

            var response = await httpClient.PostAsJsonAsync(_aiSettings.Endpoint, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get AI response: {response.StatusCode}");
            }

            var content = await response.Content.ReadFromJsonAsync<AiChatToolResponse>();
            var message = content?.Choices?.FirstOrDefault()?.Message;

            // If the model wants to call a tool:
            if (message?.ToolCalls != null && message.ToolCalls.Any())
            {
                return await HandleToolCallsAsync(message.ToolCalls, systemPrompt, userPrompt, message);
            }

            return message?.Content?.Trim() ?? "";
        }

        private async Task<string> SendToolResponseToAi(ToolCall toolCall, AiChatToolMessage message, string systemPrompt, string userPrompt, object toolResult)
        {
            var httpClient = CreateHttpClient();

            var followUpBody = new
            {
                model = _aiSettings.Model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt },
                    new { role = "assistant", tool_calls = message.ToolCalls },
                    new { role = "tool", tool_call_id = toolCall.Id, content = JsonSerializer.Serialize(toolResult) }
                }
            };

            var response = await httpClient.PostAsJsonAsync(_aiSettings.Endpoint, followUpBody);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get AI response after tool call: {response.StatusCode}");
            }

            var finalContent = await response.Content.ReadFromJsonAsync<AiChatToolResponse>();
            return finalContent?.Choices?.FirstOrDefault()?.Message?.Content?.Trim() ?? string.Empty;
        }

        private async Task<string> HandleToolCallsAsync(List<ToolCall> toolCalls, string systemPrompt, string userPrompt, AiChatToolMessage assistantMessage)
        {
            foreach (var toolCall in toolCalls)
            {
                switch (toolCall.Function.Name)
                {
                    case "search_books":
                        return await HandleSearchBooksToolAsync(toolCall, systemPrompt, userPrompt, assistantMessage);

                    case "get_all_books":
                        return await HandleGetAllBooksAsync(toolCall, systemPrompt, userPrompt, assistantMessage);

                    default:
                        throw new NotSupportedException($"Tool '{toolCall.Function.Name}' is not supported.");
                }
            }

            return string.Empty;
        }


        private async Task<string> HandleSearchBooksToolAsync(ToolCall toolCall, string systemPrompt, string userPrompt, AiChatToolMessage message)
        {
            var args = JsonSerializer.Deserialize<BookSearch>(toolCall.Function.Arguments);

            if (args == null || (string.IsNullOrWhiteSpace(args.Title) && string.IsNullOrWhiteSpace(args.Author)))
            {
                throw new Exception("Invalid arguments for search_books tool");
            }

            var books = await _bookService.SearchBooksAsync(args);
            return await SendToolResponseToAi(toolCall, message, systemPrompt, userPrompt, books);
        }

        private async Task<string> HandleGetAllBooksAsync(ToolCall toolCall, string systemPrompt, string userPrompt, AiChatToolMessage message)
        {
            var books = await _bookService.GetAllBooksAsync();
            return await SendToolResponseToAi(toolCall, message, systemPrompt, userPrompt, books);
        }

        private HttpClient CreateHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            // Increase timeout: response from LLM can take more than 100s (default value)
            client.Timeout = TimeSpan.FromSeconds(300);
            return client;
        }
    }
}
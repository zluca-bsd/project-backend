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
            var systemPrompt = """
            You are a library assistant that answers user requests.
            - Do not use general knowledge.
            - Always use tools for book information.
            - Use the information provided by tools if present.
            - If the book exists in the database, trust and return that data.
            - Do not contradict the database.
            - Keep your answers clear and factual.
            - Always elaborate the answer in a human readable format.
            - Do not include notes.
            """;

            List<object> chatHistory =
            [
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt },
            ];

            var httpClient = _httpClientFactory.CreateClient();
            // Increase timeout: response from LLM can take more than 100s for complex request (default value)
            httpClient.Timeout = TimeSpan.FromSeconds(1200);
            string finalAnswer;

            // The loop allows multiple round toolcalls
            while (true)
            {
                var requestBody = new
                {
                    model = _aiSettings.Model,
                    messages = chatHistory,
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
                    await HandleToolCallsAsync(message.ToolCalls, chatHistory, message);
                    // Loop again after executing tool calls
                    continue;
                }

                // Otherwise, final message is reached
                finalAnswer = message?.Content?.Trim() ?? "";
                break;
            }

            return new AiResponse { Response = finalAnswer };
        }

        private async Task HandleToolCallsAsync(List<ToolCall> toolCalls, List<object> chatHistory, AiChatToolMessage assistantMessage)
        {
            // Add assistant's tool calls to chat history
            chatHistory.Add(new { role = "assistant", tool_calls = assistantMessage.ToolCalls });

            foreach (var toolCall in toolCalls)
            {
                object toolResult;

                switch (toolCall.Function.Name)
                {
                    case "search_books":
                        toolResult = await HandleSearchBooksToolAsync(toolCall);
                        break;

                    case "get_all_books":
                        toolResult = await HandleGetAllBooksAsync();
                        break;

                    case "get_book_by_id":
                        toolResult = await HandleGetBookByIdAsync(toolCall);
                        break;

                    case "create_book":
                        toolResult = await HandleCreateBookAsync(toolCall);
                        break;

                    case "update_book":
                        toolResult = await HandleUpdateBookAsync(toolCall);
                        break;

                    case "delete_book":
                        toolResult = await HandleDeleteBookAsync(toolCall);
                        break;

                    default:
                        throw new NotSupportedException($"Tool '{toolCall.Function.Name}' is not supported.");
                }

                // Add the tool response to chat history for each tool call
                chatHistory.Add(new { role = "tool", tool_call_id = toolCall.Id, content = JsonSerializer.Serialize(toolResult) });
            }
        }

        private async Task<object> HandleSearchBooksToolAsync(ToolCall toolCall)
        {
            var args = JsonSerializer.Deserialize<BookSearch>(toolCall.Function.Arguments);

            if (args == null || (string.IsNullOrWhiteSpace(args.Title) && string.IsNullOrWhiteSpace(args.Author)))
            {
                return "Invalid arguments for search_books tool";
            }

            return await _bookService.SearchBooksAsync(args);
        }

        private async Task<object> HandleGetAllBooksAsync()
        {
            return await _bookService.GetAllBooksAsync();
        }

        private async Task<object> HandleGetBookByIdAsync(ToolCall toolCall)
        {
            var bookIdDto = JsonSerializer.Deserialize<BookIdDto>(toolCall.Function.Arguments);

            if (bookIdDto == null || !Guid.TryParse(bookIdDto.Id, out Guid id))
            {
                return "the book Id is not valid";
            }

            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                return "book is not found";
            }

            return book;
        }

        private async Task<object> HandleCreateBookAsync(ToolCall toolCall)
        {
            var book = JsonSerializer.Deserialize<BookCreateDto>(toolCall.Function.Arguments);

            if (book == null)
            {
                return "The book is not valid";
            }

            return await _bookService.CreateBookAsync(book);
        }

        private async Task<object> HandleUpdateBookAsync(ToolCall toolCall)
        {
            var book = JsonSerializer.Deserialize<BookUpdateDto>(toolCall.Function.Arguments);

            if (book == null)
            {
                return "The book is not valid";
            }

            var result = await _bookService.UpdateBookAsync(book.Id, book);

            if (result == null)
            {
                return "The book is not valid";
            }

            return result;
        }

        private async Task<object> HandleDeleteBookAsync(ToolCall toolCall)
        {
            var bookIdDto = JsonSerializer.Deserialize<BookIdDto>(toolCall.Function.Arguments);

            if (bookIdDto == null || !Guid.TryParse(bookIdDto.Id, out Guid id))
            {
                return "the book Id is not valid";
            }

            var deleted = await _bookService.DeleteBookAsync(id);

            return deleted ? "Book has been deleted" : "Book not found";
        }
    }
}
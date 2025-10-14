using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using project_backend.Dtos.AiDtos;
using project_backend.Models.BookModels;
using project_backend.Services.Interfaces;

namespace project_backend.Services
{
    public class AiService : IAiService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AiSettings _aiSettings;

        public AiService(AppDbContext dbContext, IHttpClientFactory httpClientFactory, IOptions<AiSettings> aiSettings)
        {
            _context = dbContext;
            _httpClientFactory = httpClientFactory;
            _aiSettings = aiSettings.Value;
        }

        public async Task<string> AskAi<T>(string request) where T : class
        {
            var systemPrompt = $"""
            You are an assistant that translates natural language questions into SQL SELECT queries.

            Schema:
            {GetDatabaseSchema()}

            Only respond with a valid SQLite SQL SELECT query. 
            Do not include any explanation. 
            Example:
            SELECT * FROM Books WHERE Title LIKE 'Animal Farm'
            Question: "{request}"

            SQL:
            """;

            var httpClient = _httpClientFactory.CreateClient();
            // Increase timeout: response from LLM can take more than 100s (default value)
            httpClient.Timeout = TimeSpan.FromSeconds(300);
            
            var response = await httpClient.PostAsJsonAsync(_aiSettings.Endpoint, new
            {
                prompt = systemPrompt,
                max_tokens = _aiSettings.MaxTokens,
                temperature = _aiSettings.Temperature,
                stop = new[] { ";" }
            });

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to get AI response");

            var content = await response.Content.ReadFromJsonAsync<AiResponse>();

            var sqlQuery = content?.Choices?.FirstOrDefault()?.Text?.Trim();

            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new Exception("AI did not return a SQL query");
            }

            // Only allow SELECT statements
            if (!sqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only SELECT queries are allowed.");
            }

            // Sanitize dangerous patterns
            if (Regex.IsMatch(sqlQuery, @"\b(INSERT|UPDATE|DELETE|DROP|ALTER|TRUNCATE)\b", RegexOptions.IgnoreCase))
            {
                throw new InvalidOperationException("Unsafe SQL query detected.");
            }

            try
            {
                var Sqlresult = await _context.Set<T>().FromSqlRaw(sqlQuery).ToListAsync();

                return await GenerateNaturalLanguageResponse(request, Sqlresult);
            }
            catch (Exception e)
            {
                throw new Exception("SQL execution failed: " + e.Message, e);
            }
        }

        private string GetDatabaseSchema()
        {
            var schema = new StringBuilder();

            foreach (var entity in _context.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                var columns = entity.GetProperties()
                    .Select(p => $"{p.Name} {MapToSqlType(p.ClrType)}");

                schema.AppendLine($"Table: {tableName}({string.Join(", ", columns)})");
            }

            Console.WriteLine(schema);
            return schema.ToString();
        }

        private string MapToSqlType(Type type)
        {
            if (type == typeof(int) || type == typeof(long)) return "INTEGER";
            if (type == typeof(string)) return "TEXT";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(bool)) return "BOOLEAN";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "REAL";
            return "TEXT"; // default fallback
        }

        private async Task<string> GenerateNaturalLanguageResponse<T>(string request, List<T> sqlresult) where T : class
        {
            var systemPrompt = $"""
            You are an assistant that answers the question based on JSON input
            Just answer the question and keep it short. Do not include any explanation

            Request: {request}

            JSON input: 
            {JsonSerializer.Serialize(sqlresult, new JsonSerializerOptions { WriteIndented = true })}
            """;

            var httpClient = _httpClientFactory.CreateClient();
            // Increase timeout: response from LLM can take more than 100s (default value)
            httpClient.Timeout = TimeSpan.FromSeconds(300);

            var response = await httpClient.PostAsJsonAsync(_aiSettings.Endpoint, new
            {
                prompt = systemPrompt,
                max_tokens = _aiSettings.MaxTokens,
                temperature = _aiSettings.Temperature,
            });

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to get AI response");

            var content = await response.Content.ReadFromJsonAsync<AiResponse>();

            var answer = content?.Choices?.FirstOrDefault()?.Text?.Trim();
            if (string.IsNullOrEmpty(answer))
            {
                throw new Exception("AI did not return a valid answer");
            }

            return answer;
        }
    }
}
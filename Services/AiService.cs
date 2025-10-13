using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using project_backend.Dtos.AiDtos;
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

        public async Task<List<T>> AskAi<T>(string request) where T : class
        {
            var systemPrompt = $"""
            You are an assistant that translates natural language questions into SQL SELECT queries.

            Schema:
            {getDatabaseSchema()}

            Only respond with a valid SQLite SQL SELECT query. 
            Do not include any explanation. 
            Example:
            SELECT * FROM Books WHERE Title Like = 'Animal Farm'
            Question: "{request}"

            SQL:
            """;

            var httpClient = _httpClientFactory.CreateClient();
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
                return await _context.Set<T>().FromSqlRaw(sqlQuery).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception("SQL execution failed: " + e.Message, e);
            }
        }

        private string getDatabaseSchema()
        {
            string databaseSchema = "";

            var entityTypes = _context.Model.GetEntityTypes();

            foreach (var et in entityTypes)
            {
                databaseSchema += et.GetTableName() + ": ";

                foreach (var prop in et.GetProperties())
                {
                    string name = prop.Name;
                    string type = prop.ClrType.Name;

                    databaseSchema += name + " " + type + ", ";
                }

                databaseSchema += "\n";
            }
            
            return databaseSchema;
        }
    }
}
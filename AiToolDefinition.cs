using System.Text.Json.Serialization;

namespace project_backend
{
    public class AiToolDefinition
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }
        [JsonPropertyName("function")]
        public required AiToolFunction Function { get; set; }
    }

    public class AiToolFunction
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("description")]
        public required string Description { get; set; }
        [JsonPropertyName("parameters")]
        public required object Parameters { get; set; }
    }
}
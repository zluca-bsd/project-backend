using System.Text.Json.Serialization;

namespace project_backend.Dtos.AiDtos
{
    public class AiChatToolResponse
    {
        public List<AiChatToolChoice>? Choices { get; set; }
    }

    public class AiChatToolChoice
    {
        public AiChatToolMessage? Message { get; set; }
    }

    public class AiChatToolMessage
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("tool_calls")]
        public List<ToolCall>? ToolCalls { get; set; }
    }

    public class ToolCall
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required ToolFunction Function { get; set; }
    }

    public class ToolFunction
    {
        public required string Name { get; set; }
        public required string Arguments { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace project_backend.Dtos.BookDtos
{
    public class BookIdDto
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }
}
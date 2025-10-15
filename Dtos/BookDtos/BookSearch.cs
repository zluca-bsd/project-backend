using System.Text.Json.Serialization;

namespace project_backend.Dtos.BookDtos
{
    public class BookSearch
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("author")]
        public string? Author { get; set; }
    }
}
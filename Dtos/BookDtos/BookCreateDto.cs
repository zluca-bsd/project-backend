using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace project_backend.Dtos.BookDtos
{
    public class BookCreateDto
    {
        [Required]
        [JsonPropertyName("title")]
        public required string Title { get; set; }
        [Required]
        [JsonPropertyName("author")]
        public required string Author { get; set; }
        [Required]
        [JsonPropertyName("price")]
        [Range(0f, float.MaxValue, ErrorMessage = "The price cannot be negative") ]
        public required float Price { get; set; }
        [Required]
        [JsonPropertyName("category")]
        public required string Category { get; set; }
    }
}
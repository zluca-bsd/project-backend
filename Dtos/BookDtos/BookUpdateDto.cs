using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.BookDtos
{
    public class BookUpdateDto
    {
        [Required]
        public required Guid Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }
        [Required]
        [Range(0f, float.MaxValue, ErrorMessage = "The price cannot be negative")]
        public required float Price { get; set; }
        [Required]
        public required string Category { get; set; }
    }
}
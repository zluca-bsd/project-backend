using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.AiDtos
{
    public class AiRequest
    {
        [Required]
        public required string Request { get; set; }
    }
}
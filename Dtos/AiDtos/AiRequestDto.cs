using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.AiDtos
{
    public class AiRequestDto
    {
        [Required]
        public required string Prompt { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.AiDtos
{
    public class AiQuestion
    {
        [Required]
        public required string Question { get; set; }
    }
}
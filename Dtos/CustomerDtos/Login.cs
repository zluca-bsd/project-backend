using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.CustomerDtos
{
    public class LoginDto
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.CustomerDtos
{
    public class CustomerUpdateDto
    {
        [Required]
        public required Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string DateOfBirth { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
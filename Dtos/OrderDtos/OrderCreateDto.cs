using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.OrderDtos
{
    public class OrderCreateDto
    {
        [Required]
        public required Guid CustomerId { get; set; }
        [Required]
        public required Guid BookId { get; set; }
    }
}
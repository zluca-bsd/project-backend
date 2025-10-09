using System.ComponentModel.DataAnnotations;

namespace project_backend.Dtos.OrderDtos
{
    public class OrderUpdateDto
    {
        [Required]
        public required Guid Id { get; set; }
        [Required]
        public required Guid CustomerId { get; set; }
        [Required]
        public required Guid BookId { get; set; }
    }
}
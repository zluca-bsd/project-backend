using System.ComponentModel.DataAnnotations;

namespace project_backend.Models.OrderModels
{
    public class Order
    {
        public Guid Id { get; set; }
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid BookId { get; set; }

        public Order(Guid customerId, Guid bookId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            BookId = bookId;
        }
    }
}
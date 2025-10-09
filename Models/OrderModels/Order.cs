namespace project_backend.Models.OrderModels
{
    public class Order
    {
        public required Guid Id { get; set; }
        public required Guid CustomerId { get; set; }
        public required Guid BookId { get; set; }
    }
}
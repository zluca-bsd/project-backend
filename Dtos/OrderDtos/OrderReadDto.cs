namespace project_backend.Dtos.OrderDtos
{
    public class OrderReadDto
    {
        public required Guid Id { get; set; }
        public required Guid CustomerId { get; set; }
        public required Guid BookId { get; set; }
    }
}
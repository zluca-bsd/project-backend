namespace project_backend.Dtos.OrderDtos
{
    public class OrderSearch
    {
        public Guid? CustomerId { get; set; }
        public Guid? BookId { get; set; }
    }
}
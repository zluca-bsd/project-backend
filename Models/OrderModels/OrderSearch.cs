namespace project_backend.Models.OrderModels
{
    public class OrderSearch
    {
        public Guid? CustomerId { get; set; }
        public Guid? BookId { get; set; }
    }
}
namespace project_backend.Models.BookModels
{
    public class Book
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required float Price { get; set; }
        public required string Category { get; set; }
    }
}
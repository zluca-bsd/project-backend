namespace project_backend.Dtos.BookDtos
{
    public class BookReadDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required float Price { get; set; }
        public required string Category { get; set; }
    }
}
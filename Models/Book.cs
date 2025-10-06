namespace project_backend.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }

        public Book(string title, string author, float price, string category)
        {
            Id = Guid.NewGuid();
            Title = title;
            Author = author;
            Price = price;
            Category = category;
        }
    }
}
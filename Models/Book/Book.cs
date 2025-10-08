using System.ComponentModel.DataAnnotations;

namespace project_backend.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
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
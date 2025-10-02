namespace project_backend
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }

        public Product(string name, float price, string category)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            Category = category;
        }
    }
}
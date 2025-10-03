namespace project_backend.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DateOfBirth { get; set; }
        public Customer(string name, string email, string dateOfBirth)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
        }
    }
}
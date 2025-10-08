namespace project_backend.Models.CustomerModels
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DateOfBirth { get; set; }
        public string Password { get; set; }

        public Customer() { }

        public Customer(string name, string email, string dateOfBirth, string password, bool isHashed = false)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;

            if (isHashed)
            {
                Password = password;  // already hashed
            }
            else
            {
                Password = BCrypt.Net.BCrypt.HashPassword(password);  // hash now
            }

        }
    }
}
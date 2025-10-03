namespace project_backend.Models
{
    public class Register
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string DateOfBirth { get; set; }
        public string Password { get; set; }

        public Register(string name, string email, string dateOfBirth, string password)
        {
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
            Password = password;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_backend
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
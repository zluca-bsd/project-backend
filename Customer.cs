using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_backend
{
    public class Customer
    {

        public Guid Id { get; }
        public string Name { get; set; }
        public float Email { get; set; }
        public string DateOfBirth { get; set; }
        public Customer(Guid id, string name, float email, string dateOfBirth)
        {
            Id = id;
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
        }

        
    }
}
using Microsoft.AspNetCore.Mvc;
using project_backend.Models;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register([FromBody] Register register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid client request");
            }

            // Check if the email already exists (case-insensitive)
            var existingUser = _context.Customers.FirstOrDefault(c => c.Email.ToLower() == register.Email.ToLower());

            if (existingUser != null)
            {
                return BadRequest("User is already registered.");
            }

            var newCustomer = new Customer(register.Name, register.Email, register.DateOfBirth, register.Password);

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
            
            return Ok("Registration successful.");
        }

    }
}
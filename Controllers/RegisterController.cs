using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Models;
using project_backend.Models.CustomerModels;

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

        /// <summary>
        /// Registers a new customer account.
        /// </summary>
        /// <param name="register">The registration details including name, email, date of birth, and password.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> - Registration was successful</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> - User with the given email is already registered</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Registration was successful</response>
        /// <response code="400">User with the given email is already registered</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            // Check if the email already exists (case-insensitive)
            var existingUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == register.Email.ToLower());

            if (existingUser != null)
            {
                return BadRequest("User is already registered.");
            }

            var newCustomer = new Customer(register.Name, register.Email, register.DateOfBirth, register.Password, false);

            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }
    }
}
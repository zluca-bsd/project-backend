using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        // GET: api/customers/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<Customer> GetCustomer(Guid id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound("Customer not found");
            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure ID is generated server-side
            customer.Id = Guid.NewGuid();

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCustomer(Guid id)
        {
            var customer = _context.Customers.Find(id);

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return NoContent();
        }

        // PUT: api/customers/{id}
        [HttpPut("{id:guid}")]
        public IActionResult UpdateCustomer(Guid Id, [FromBody] Customer customer)
        {
            // Ensure the route ID and body ID match
            if (Id != customer.Id)
            {
                return BadRequest("The ID in the URL does not match the customer ID.");
            }

            // Check if the customers exists
            var existingCustomer = _context.Customers.Find(Id);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found");
            }

            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.DateOfBirth = customer.DateOfBirth;

            try
            {
                _context.Customers.Update(existingCustomer);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, $"An error occurred while updating the customer: {e.Message}");
            }

            return Ok(existingCustomer);
        }
    }
}
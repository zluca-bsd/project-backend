using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace project_backend
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

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        [HttpGet("{Id}")]
        public ActionResult<Customer> GetCustomer(Guid Id)
        {
            var customer = _context.Customers.Find(Id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customer);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpDelete("{Id}")]
        public IActionResult DeleteCustomer(Guid Id)
        {
            var customer = _context.Customers.Find(Id);

            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{Id}")]
        public IActionResult PutCustomer(Guid Id, [FromBody] Customer customer)
        {
            // Ensure the route ID and body ID match
            if (Id != customer.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
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
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }

            return Ok(existingCustomer);
        }
    }
}
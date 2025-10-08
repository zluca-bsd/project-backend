using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Models;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        /// <summary>
        /// Retrieves all customers from the database.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> - Returns the list of customers.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Returns the list of customers</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        // GET: api/customers/{id}
        /// <summary>
        /// Retrieves a specific customer by their unique identifier (GUID).
        /// </summary>
        /// <param name="id">The GUID of the customer to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> - Returns the customer with the specified ID</description></item>
        ///   <item><description><see cref="StatusCodes.Status404NotFound"/> - If the customer is not found</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Returns the customer with the specified ID</response>
        /// <response code="404">If the customer is not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Customer> GetCustomer(Guid id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound("Customer not found");
            return Ok(customer);
        }

        // // POST: api/customers
        // Disabled: use /register from RegisterController.cs instead
        // [HttpPost]
        // public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     // Ensure ID is generated server-side
        //     customer.Id = Guid.NewGuid();

        //     _context.Customers.Add(customer);
        //     _context.SaveChanges();

        //     return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        // }

        // DELETE: api/customers/{id}
        /// <summary>
        /// Deletes a customer by their unique identifier (GUID).
        /// </summary>
        /// <param name="id">The GUID of the customer to delete.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status204NoContent"/> - Customer was successfully deleted</description></item>
        ///   <item><description><see cref="StatusCodes.Status404NotFound"/> - Customer with the specified ID was not found</description></item>
        /// </list>
        /// </returns>
        /// <response code="204">Customer was successfully deleted</response>
        /// <response code="404">Customer with the specified ID was not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Updates an existing customer by their unique identifier (GUID).
        /// </summary>
        /// <param name="Id">The GUID of the customer to update (from the route).</param>
        /// <param name="customer">The updated customer object (from the request body).</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> - Customer was successfully updated</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> - The ID in the URL does not match the customer ID</description></item>
        ///   <item><description><see cref="StatusCodes.Status404NotFound"/> - Customer with the specified ID was not found</description></item>
        ///   <item><description><see cref="StatusCodes.Status500InternalServerError"/> - An error occurred during the update</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Customer was successfully updated</response>
        /// <response code="400">The ID in the URL does not match the customer ID</response>
        /// <response code="404">Customer with the specified ID was not found</response>
        /// <response code="500">An error occurred during the update</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
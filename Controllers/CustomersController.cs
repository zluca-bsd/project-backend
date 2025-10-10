using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.Dtos.CustomerDtos;
using project_backend.Services.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
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
        public async Task<ActionResult<List<CustomerReadDto>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
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
        public async Task<ActionResult<CustomerReadDto>> GetCustomerById(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

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
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);

            if (!deleted)
            {
                return NotFound("Customer not found");
            }

            return NoContent();
        }

        // PUT: api/customers/{id}
        /// <summary>
        /// Updates an existing customer by their unique identifier (GUID).
        /// </summary>
        /// <param name="id">The GUID of the customer to update (from the route).</param>
        /// <param name="customerUpdateDto">The updated customer object (from the request body).</param>
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
        public async Task<ActionResult<CustomerReadDto>> UpdateCustomer(Guid id, [FromBody] CustomerUpdateDto customerUpdateDto)
        {
            // Ensure the route ID and body ID match
            if (id != customerUpdateDto.Id)
            {
                return BadRequest("The ID in the URL does not match the customer ID.");
            }

            // Check if the customers exists
            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerUpdateDto);
            if (updatedCustomer == null)
            {
                return NotFound("Customer not found");
            }

            return Ok(updatedCustomer);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace project_backend
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize]
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders.ToList();
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id:guid}"), Authorize]
        public ActionResult<Order> GetOrder(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound("Order not found");
            return Ok(order);
        }

        // POST: api/orders
        [HttpPost, Authorize]
        public ActionResult<Order> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure ID is generated server-side
            order.Id = Guid.NewGuid();

            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id:guid}"), Authorize]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }

        // PUT: api/orders/{id}
        [HttpPut("{id:guid}"), Authorize]
        public IActionResult UpdateOrder(Guid id, [FromBody] Order order)
        {
            // Ensure the route ID and body ID match
            if (id != order.Id)
            {
                return BadRequest("The ID in the URL does not match the order ID.");
            }

            // Check if the order exists
            var existingOrder = _context.Orders.Find(id);
            if (existingOrder == null)
            {
                return NotFound("Order not found");
            }

            // Update fields
            existingOrder.BookId = order.BookId;
            existingOrder.CustomerId = order.CustomerId;

            try
            {
                _context.Orders.Update(existingOrder);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, $"An error occurred while updating the order: {e.Message}");
            }

            return Ok(existingOrder);
        }

        // GET: api/orders/search?bookid=BookId&customerid=CustomerId
        [HttpGet("search"), Authorize]
        public ActionResult<Order> SearchBookByTitle([FromQuery] BookSearch searchCriteria)
        {
            var filteredBooks = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchCriteria.Title))
            {
                filteredBooks = filteredBooks.Where(b =>
                    EF.Functions.Like(b.Title, $"%{searchCriteria.Title}%")
                );
            }

            if (!string.IsNullOrEmpty(searchCriteria.Author))
            {
                filteredBooks = filteredBooks.Where(b =>
                    EF.Functions.Like(b.Author, $"%{searchCriteria.Author}%")
                );
            }

            var result = filteredBooks.ToList();

            if (!result.Any())
            {
                return NotFound("No books found with the given title.");
            }

            return Ok(result);
        }
    }
}
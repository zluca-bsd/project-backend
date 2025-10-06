using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Models;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        // Authenticated users only
        [HttpGet, Authorize]
        public IActionResult GetAllBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id:guid}"), Authorize]
        public ActionResult<Book> GetBook(Guid id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            return Ok(book);
        }

        // POST: api/books
        [HttpPost, Authorize]
        public ActionResult<Book> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure ID is generated server-side
            book.Id = Guid.NewGuid();

            _context.Books.Add(book);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id:guid}"), Authorize]
        public IActionResult DeleteBook(Guid id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            _context.Books.Remove(book);
            _context.SaveChanges();

            return NoContent();
        }

        // PUT: api/books/{id}
        [HttpPut("{id:guid}"), Authorize]
        public IActionResult UpdateBook(Guid id, [FromBody] Book book)
        {
            // Ensure the route ID and body ID match
            if (id != book.Id)
            {
                return BadRequest("The ID in the URL does not match the book ID.");
            }

            // Check if the book exists
            var existingBook = _context.Books.Find(id);
            if (existingBook == null)
            {
                return NotFound("Book not found");
            }

            // Update fields
            existingBook.Name = book.Name;
            existingBook.Price = book.Price;
            existingBook.Category = book.Category;

            try
            {
                _context.Books.Update(existingBook);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, $"An error occurred while updating the book: {e.Message}");
            }

            return Ok(existingBook);
        }
    }
}

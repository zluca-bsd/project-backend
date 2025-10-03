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
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        [HttpGet("{Id}")]
        public ActionResult<Book> GetBook(Guid Id)
        {
            var book = _context.Books.Find(Id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpDelete("{Id}")]
        public IActionResult DeleteBook(Guid Id)
        {
            var book = _context.Books.Find(Id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{Id}")]
        public IActionResult PutBook(Guid Id, [FromBody] Book book)
        {
            // Ensure the route ID and body ID match
            if (Id != book.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if the book exists
            var existingBook = _context.Books.Find(Id);
            if (existingBook == null)
            {
                return NotFound("Book not found");
            }

            existingBook.Name = book.Name;
            existingBook.Price = book.Price;
            existingBook.Category = book.Category;

            try
            {
                _context.Books.Update(existingBook);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }

            return Ok(existingBook);
        }
    }
}
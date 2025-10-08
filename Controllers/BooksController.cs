using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Models.BookModels;
using project_backend.Dtos.BookDtos;
using AutoMapper;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/books
        /// <summary>
        /// Retrieves all books from the database.
        /// </summary>
        /// <returns>
        /// A list of all books in the system.
        /// </returns>
        /// <response code="200">Returns the list of books.</response>
        /// <remarks>
        /// This endpoint requires the user to be authenticated.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookReadDto>>> GetAllBooks()
        {
            var books = await _context.Books.ToListAsync();
            var bookReadDtos = _mapper.Map<IEnumerable<BookReadDto>>(books);

            return Ok(bookReadDtos);
        }

        // GET: api/books/{id}
        /// <summary>
        /// Retrieves a specific book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the book to retrieve.</param>
        /// <returns>
        /// Returns the book details if found.
        /// </returns>
        /// <response code="200">Returns the requested book.</response>
        /// <response code="404">If a book with the specified ID is not found.</response>
        /// <remarks>
        /// This endpoint requires authentication.
        /// </remarks>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookReadDto>> GetBook(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            var bookReadDto = _mapper.Map<BookReadDto>(book);

            return Ok(bookReadDto);
        }

        // POST: api/books
        /// <summary>
        /// Creates the book in the database
        /// </summary>
        /// <param name="bookCreateDto">The book that needs to be created</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status201Created"/> – If the book was successfully created.</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> – If the book parameter contains error.</description></item>
        /// </list>
        /// </returns>
        /// <response code="201">The book has been successfully created.</response>
        /// <response code="400">The book parameter contains error.</response>
        /// <remarks>
        /// This endpoint requires authorization.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookCreateDto>> CreateBook([FromBody] BookCreateDto bookCreateDto)
        {
            var book = _mapper.Map<Book>(bookCreateDto);
            // Ensure ID is generated server-side
            book.Id = Guid.NewGuid();

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            var bookReadDto = _mapper.Map<BookReadDto>(book);

            return CreatedAtAction(nameof(GetBook), new { id = bookReadDto.Id }, bookReadDto);
        }

        // DELETE: api/books/{id}
        /// <summary>
        /// Deletes a book from the database by its ID.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the book to delete.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status204NoContent"/> – If the book was successfully deleted.</description></item>
        ///   <item><description><see cref="StatusCodes.Status404NotFound"/> – If the book with the specified ID was not found.</description></item>
        /// </list>
        /// </returns>
        /// <response code="204">The book has been successfully deleted.</response>
        /// <response code="404">No book with the specified ID was found.</response>
        /// <remarks>
        /// This endpoint requires authorization.
        /// </remarks>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/books/{id}
        /// <summary>
        /// Updates the details of an existing book by its ID.
        /// </summary>
        /// <param name="id">The id of the book to update</param>
        /// <param name="bookUpdateDto">The updated book object containing new values</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> – If the book was successfully updated. Returns the updated book.</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> – If the ID in the URL does not match the ID in the book object.</description></item>
        ///   <item><description><see cref="StatusCodes.Status404NotFound"/> – If the book with the specified ID does not exist.</description></item>
        ///   <item><description><see cref="StatusCodes.Status500InternalServerError"/> – If an error occurred while updating the book in the database.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This endpoint requires authorization.
        /// </remarks>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            // Ensure the route ID and body ID match
            if (id != bookUpdateDto.Id)
            {
                return BadRequest("The ID in the URL does not match the book ID.");
            }

            // Check if the book exists
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                return NotFound("Book not found");
            }

            // Update fields with automapper
            _mapper.Map(bookUpdateDto, existingBook);

            try
            {
                // Entity is already tracked, no need to call _context.Update();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, $"An error occurred while updating the book: {e.Message}");
            }

            var bookReadDto = _mapper.Map<BookReadDto>(existingBook);

            return Ok(bookReadDto);
        }

        // GET: api/books/search?title=Title&author=Author
        /// <summary>
        /// Searches for books based on optional title and/or author criteria.
        /// </summary>
        /// <param name="searchCriteria">An object containing optional title and author search parameters.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> – If the books were successfully found. Returns the list of books.</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> – If the search criteria is invalid (empty).</description></item>
        ///   <item><description><see cref="StatusCodes.Status404NotFound"/> – If No books matched the search criteria.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Returns the list of matching books.</response>
        /// <response code="400">The search criteria is invalid.</response>
        /// <response code="404">No books matched the search criteria.</response>
        /// <remarks>
        /// This endpoint requires authentication.
        /// </remarks>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<BookReadDto>>> SearchBook([FromQuery] BookSearch searchCriteria)
        {
            if (string.IsNullOrWhiteSpace(searchCriteria.Title) && string.IsNullOrWhiteSpace(searchCriteria.Author))
            {
                return BadRequest("At least one of 'title' or 'author' must be provided for search.");
            }

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

            var books = await filteredBooks.ToListAsync();

            if (!books.Any())
            {
                return NotFound("No books found with the given search criteria.");
            }

            var bookReadDto = _mapper.Map<List<BookReadDto>>(books);

            return Ok(bookReadDto);
        }
    }
}

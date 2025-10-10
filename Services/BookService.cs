using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project_backend.Dtos.BookDtos;
using project_backend.Models.BookModels;
using project_backend.Services.Interfaces;

namespace project_backend.Services
{
    public class BookService : IBooksService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BookService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookReadDto>> GetAllBooksAsync()
        {
            var books = await _context.Books.ToListAsync();
            return _mapper.Map<List<BookReadDto>>(books);
        }

        public async Task<BookReadDto?> GetBookByIdAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            return _mapper.Map<BookReadDto>(book);
        }

        public async Task<BookReadDto> CreateBookAsync(BookCreateDto dto)
        {
            var book = _mapper.Map<Book>(dto);
            // Ensure ID is generated server-side
            book.Id = Guid.NewGuid();

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return _mapper.Map<BookReadDto>(book);
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<BookReadDto?> UpdateBookAsync(Guid id, BookUpdateDto dto)
        {
            // Ensure the route ID and body ID match
            if (id != dto.Id)
            {
                return null;
            }

            // Check if the book exists
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            // Update fields with automapper
            _mapper.Map(dto, book);

            try
            {
                // Entity is already tracked, no need to call _context.Update();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException(e.Message);
            }

            return _mapper.Map<BookReadDto>(book);
        }

        public async Task<List<BookReadDto>> SearchBooksAsync(BookSearch searchCriteria)
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

            var books = await filteredBooks.ToListAsync();
            return _mapper.Map<List<BookReadDto>>(books);
        }
    }
}
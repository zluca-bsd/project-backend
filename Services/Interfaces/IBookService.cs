using project_backend.Dtos.BookDtos;

namespace project_backend.Services.Interfaces
{
    public interface IBooksService
    {
        Task<List<BookReadDto>> GetAllBooksAsync();
        Task<BookReadDto?> GetBookByIdAsync(Guid id);
        Task<BookReadDto> CreateBookAsync(BookCreateDto dto);
        Task<bool> DeleteBookAsync(Guid id);
        Task<BookReadDto?> UpdateBookAsync(Guid id, BookUpdateDto dto);
        Task<List<BookReadDto>> SearchBooksAsync(BookSearch searchCriteria);
    }
}
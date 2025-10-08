using AutoMapper;
using project_backend.Dtos.BookDtos;
using project_backend.Models.BookModels;

namespace project_backend.Mappings
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            // For returning data
            CreateMap<Book, BookReadDto>();
            // For creating new books
            CreateMap<BookCreateDto, Book>();
            // Optional: For updating
            CreateMap<BookUpdateDto, Book>();
        }
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project_backend.Dtos.CustomerDtos;
using project_backend.Models.CustomerModels;

namespace project_backend.Services.Interfaces
{
    public class RegisterService : IRegisterService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RegisterService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            dto.Name = dto.Name.Trim();
            dto.Email = dto.Email.Trim().ToLower();

            // Check if the email already exists (case-insensitive)
            var existingUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (existingUser != null)
            {
                return false;
            }

            var newCustomer = _mapper.Map<Customer>(dto);
            // Ensure ID is generated server-side
            newCustomer.Id = Guid.NewGuid();
            // Hash the password
            newCustomer.Password = BCrypt.Net.BCrypt.HashPassword(newCustomer.Password);

            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
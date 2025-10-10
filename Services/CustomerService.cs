using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project_backend.Dtos.CustomerDtos;
using project_backend.Services.Interfaces;

namespace project_backend.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CustomerReadDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.ToListAsync();

            return _mapper.Map<List<CustomerReadDto>>(customers);
        }

        public async Task<CustomerReadDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return null;
            }

            return _mapper.Map<CustomerReadDto>(customer);
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CustomerReadDto?> UpdateCustomerAsync(Guid id, CustomerUpdateDto dto)
        {
            // Check if the customers exists
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return null;
            }

            dto.Name = dto.Name.Trim();
            dto.Email = dto.Email.Trim().ToLower();

            // Update fields with automapper
            _mapper.Map(dto, existingCustomer);

            try
            {
                // Entity is already tracked, no need to call _context.Update();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException(e.Message);
            }

            return _mapper.Map<CustomerReadDto>(existingCustomer);
        }
    }
}
using project_backend.Dtos.CustomerDtos;

namespace project_backend.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerReadDto>> GetAllCustomersAsync();
        Task<CustomerReadDto?> GetCustomerByIdAsync(Guid id);
        Task<CustomerReadDto?> UpdateCustomerAsync(Guid id, CustomerUpdateDto dto);
        Task<bool> DeleteCustomerAsync(Guid id);
    }
}
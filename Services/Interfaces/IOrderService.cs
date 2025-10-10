using Microsoft.AspNetCore.Mvc;
using project_backend.Dtos.OrderDtos;

namespace project_backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderReadDto>> GetAllOrdersAsync();
        Task<OrderReadDto?> GetOrderByIdAsync(Guid id);
        Task<(bool Success, string Message, OrderReadDto? Order)> CreateOrderAsync(OrderCreateDto dto);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<(bool Success, string Message, OrderReadDto? Order)> UpdateOrderAsync(Guid id, OrderUpdateDto dto);
        Task<(bool Success, string Message, List<OrderReadDto>? Orders)> SearchOrdersAsync(OrderSearch searchCriteria);
    }
}
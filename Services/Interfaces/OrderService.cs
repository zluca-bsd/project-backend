using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project_backend.Dtos.OrderDtos;
using project_backend.Models.OrderModels;

namespace project_backend.Services.Interfaces
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OrderReadDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            return _mapper.Map<List<OrderReadDto>>(orders);
        }

        public async Task<OrderReadDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return null;
            }

            return _mapper.Map<OrderReadDto>(order);
        }

        public async Task<(bool Success, string Message, OrderReadDto? Order)> CreateOrderAsync(OrderCreateDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);

            if (customer == null)
            {
                return (false, "CustomerId does not exist", null);
            }

            var book = await _context.Books.FindAsync(dto.BookId);

            if (book == null)
            {
                return (false, "BookId does not exist", null);
            }

            var order = _mapper.Map<Order>(dto);

            // Ensure ID is generated server-side
            order.Id = Guid.NewGuid();

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderReadDto = _mapper.Map<OrderReadDto>(order);

            return (false, "Order created successfully", orderReadDto);
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(bool Success, string Message, OrderReadDto? Order)> UpdateOrderAsync(Guid id, OrderUpdateDto dto)
        {
            // Ensure the route ID and body ID match
            if (id != dto.Id)
            {
                return (false, "The ID in the URL does not match the order ID.", null);
            }

            // Check if the order exists
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return (false, "Order not found", null);
            }

            // Check if the customer and book exists
            var newCustomer = await _context.Customers.FindAsync(dto.CustomerId);
            if (newCustomer == null)
            {
                return (false, "The customer does not exist", null);
            }

            var newBook = await _context.Books.FindAsync(dto.BookId);
            if (newBook == null)
            {
                return (false, "The book does not exist", null);
            }

            // Update fields with automapper
            _mapper.Map(dto, existingOrder);

            try
            {
                // Entity is already tracked, no need to call _context.Update();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return (false, $"An error occurred while updating the order: {e.Message}", null);
            }

            var orderReadDto = _mapper.Map<OrderReadDto>(existingOrder);

            return (true, "Order updated successfully", orderReadDto);
        }

        public async Task<(bool Success, string Message, List<OrderReadDto>? Orders)> SearchOrdersAsync(OrderSearch searchCriteria)
        {
            if (searchCriteria.CustomerId == null && searchCriteria.BookId == null)
            {
                return (false, "At least one of 'CustomerId' or 'BookId' must be provided for search.", null);
            }

            var filteredOrders = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchCriteria.CustomerId.ToString()))
            {
                filteredOrders = filteredOrders.Where(b =>
                    EF.Functions.Like(b.CustomerId.ToString(), $"%{searchCriteria.CustomerId}%")
                );
            }

            if (!string.IsNullOrEmpty(searchCriteria.BookId.ToString()))
            {
                filteredOrders = filteredOrders.Where(b =>
                    EF.Functions.Like(b.BookId.ToString(), $"%{searchCriteria.BookId}%")
                );
            }

            var orders = await filteredOrders.ToListAsync();

            if (!orders.Any())
            {
                return (false, "No orders found with the given search criteria.", null);
            }

            var orderReadDtos = _mapper.Map<List<OrderReadDto>>(orders);

            return (true, "orders found", orderReadDtos);
        }
    }
}
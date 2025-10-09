using AutoMapper;
using project_backend.Dtos.OrderDtos;
using project_backend.Models.OrderModels;

namespace project_backend.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // For returning data
            CreateMap<Order, OrderReadDto>();
            // For order creation
            CreateMap<OrderCreateDto, Order>();
            // For order update
            CreateMap<OrderUpdateDto, Order>();
        }
    }
}
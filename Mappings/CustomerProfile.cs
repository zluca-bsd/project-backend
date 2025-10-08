using AutoMapper;
using project_backend.Dtos.CustomerDtos;
using project_backend.Models.CustomerModels;

namespace project_backend.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // For registration
            CreateMap<RegisterDto, Customer>();
            // For returning data
            CreateMap<Customer, CustomerReadDto>();
            // For updating
            CreateMap<CustomerUpdateDto, Customer>();
        }
    }
}
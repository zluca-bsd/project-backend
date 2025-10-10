using project_backend.Dtos.CustomerDtos;

namespace project_backend.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
    }
}
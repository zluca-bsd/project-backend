using project_backend.Dtos.CustomerDtos;

namespace project_backend.Services.Interfaces
{
    public interface ILoginService
    {
        Task<string?> Login(LoginDto dto);
    }
}
using project_backend.Dtos.AiDtos;

namespace project_backend.Services.Interfaces
{
    public interface IAiService
    {
        Task<AiResponseDto> GetChatResponseAsync(AiRequestDto request);
    }
}
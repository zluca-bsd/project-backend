using project_backend.Models.BookModels;

namespace project_backend.Services.Interfaces
{
    public interface IAiService
    {
        Task<string> AskAi<T>(string request) where T : class;
    }
}
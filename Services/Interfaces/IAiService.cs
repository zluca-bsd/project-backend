using project_backend.Models.BookModels;

namespace project_backend.Services.Interfaces
{
    public interface IAiService
    {
        Task<List<T>> AskAi<T>(string request) where T : class;
    }
}
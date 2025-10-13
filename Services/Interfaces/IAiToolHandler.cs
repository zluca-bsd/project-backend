namespace project_backend.Services.Interfaces
{
    public interface IAiToolHandler
    {
        string Name { get; }
        Task<string> HandleAsync(string argumentsJson);
    }
}
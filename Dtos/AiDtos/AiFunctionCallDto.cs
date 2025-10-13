namespace project_backend.Dtos.AiDtos
{
    public class AiFunctionCallDto
    {
    public required string Name { get; set; }
    public required string Arguments { get; set; } // JSON string
    }
}
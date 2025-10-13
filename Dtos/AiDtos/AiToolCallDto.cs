namespace project_backend.Dtos.AiDtos
{
    public class AiToolCallDto
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required AiFunctionCallDto Function { get; set; }
    }
}
namespace project_backend.Dtos.AiDtos
{
    public class AiResponse
    {
        public required List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public required string Text { get; set; }
    }
}
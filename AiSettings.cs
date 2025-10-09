namespace project_backend
{
    public class AiSettings
    {
        public required string Model { get; set; }   
        public required float Temperature { get; set; }
        public required float MaxTokens { get; set; }
        public required bool Stream { get; set; }
        public required string Endpoint { get; set; }
    }
}
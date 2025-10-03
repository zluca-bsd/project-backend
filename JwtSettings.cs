namespace project_backend
{
    public class JwtSettings
    {
        public required string Secret { get; set; }   
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
    }
}
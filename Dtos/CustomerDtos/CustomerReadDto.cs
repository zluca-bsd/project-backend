namespace project_backend.Dtos.CustomerDtos
{
    public class CustomerReadDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string DateOfBirth { get; set; }
    }
}
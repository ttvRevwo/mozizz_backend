namespace MozizzAPI.DTOS
{
    public class RegisterDto
    {
        public required string Name { get; set; } = null!;
        public required string Email { get; set; } = null!;
        public required string Password { get; set; } = null!;
        public required string? Phone { get; set; }
    }
}

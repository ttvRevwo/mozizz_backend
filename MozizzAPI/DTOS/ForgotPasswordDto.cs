namespace MozizzAPI.DTOS
{
    public class ForgotPasswordDto
    {
        public required string Email { get; set; }
    }
    public class ResetPasswordDto
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
        public required string NewPassword { get; set; }
    }
}

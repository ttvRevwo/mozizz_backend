namespace MozizzAPI.DTOS
{
    public class PasswordChangeDto
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}

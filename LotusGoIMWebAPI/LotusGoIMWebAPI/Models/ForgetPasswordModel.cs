namespace LotusGoIMWebAPI.Models
{
    public class ForgetPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }
}

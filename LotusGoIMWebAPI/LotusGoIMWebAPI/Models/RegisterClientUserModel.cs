using LotusGoIMWebAPI.Entities;

namespace LotusGoIMWebAPI.Models
{
    public class RegisterClientUserModel:ClientUser
    {
        public string ConfirmPassword { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }
}

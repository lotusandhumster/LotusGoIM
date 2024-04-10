using LotusGoIMWebAPI.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class ClientUser
    {
        [Key]
        public int UserId { get; set; }
        public string NickName { get; set; } = String.Empty;
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string? AvatarUrl { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Description { get; set; } = String.Empty;
        public ClientUserState State { get; set; } = ClientUserState.Offline;
        public bool IsDeleted { get; set; } = false;

    }
}

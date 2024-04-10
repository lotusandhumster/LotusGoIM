using LotusGoIMWebAPI.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class Friend
    {
        [Key]
        public int FriendId { get; set; }
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
        public string? Remark { get; set; } = string.Empty;
        public FriendType Type { get; set; } = FriendType.Friend;
        public bool IsDeleted { get; set; } = false;
    }
}

using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;

namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class FriendSearchFilter
    {
        public string? NickName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int? ClassifyId { get; set; }
        public string? Remark { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public ClientUserState? State { get; set; }
        public FriendType? Type { get; set; }
        public bool IsDeleted { get; set; }

    }
}

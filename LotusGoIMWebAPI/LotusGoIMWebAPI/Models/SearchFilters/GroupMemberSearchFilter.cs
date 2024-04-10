using LotusGoIMWebAPI.Common.Enums;

namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class GroupMemberSearchFilter
    {
        public int? GroupId { get; set; }
        public int? MemberId { get; set; }
        public string? MemberName { get; set; } = string.Empty;
        public GenderType? Gender { get; set; }
        public bool IsDeleted { get; set; }
    }
}

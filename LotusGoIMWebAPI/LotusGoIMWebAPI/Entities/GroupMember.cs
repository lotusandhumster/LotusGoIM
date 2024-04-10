using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class GroupMember
    {
        [Key]
        public int GroupMemberId { get; set; }
        public int GroupId { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}

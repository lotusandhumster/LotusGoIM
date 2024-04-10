using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int Owner { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}

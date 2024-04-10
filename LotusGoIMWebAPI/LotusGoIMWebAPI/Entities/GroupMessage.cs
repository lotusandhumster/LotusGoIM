using LotusGoIMWebAPI.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class GroupMessage
    {
        [Key]
        public int GroupMessageId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public MessageType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SendTime { get; set; }
        public string? FileUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

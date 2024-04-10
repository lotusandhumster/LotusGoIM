using LotusGoIMWebAPI.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class PrivateMessage
    {
        [Key]
        public int PrivateMessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public MessageType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SendTime { get; set; }
        public string? FileUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

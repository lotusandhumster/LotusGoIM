using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Entities
{
    public class ChatGptMessage
    {
        [Key]
        public int ChatGptMessageId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime SendTime { get; set; }
    }
}

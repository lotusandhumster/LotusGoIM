using LotusGoIMWebAPI.Common.Enums;

namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class PrivateMessageSearchFilter : PageParam
    {
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType? Type { get; set; }
    }
}

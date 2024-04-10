using LotusGoIMWebAPI.Common.Enums;

namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class GroupMessageSearchFilter : PageParam
    {
        public int? GroupId { get; set; }
        public int? ClientUserId { get; set; }
        public string? Content { get; set; } = string.Empty;
        public MessageType? Type { get; set; }
    }
}

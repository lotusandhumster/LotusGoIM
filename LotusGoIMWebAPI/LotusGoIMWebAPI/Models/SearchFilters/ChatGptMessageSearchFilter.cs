namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class ChatGptMessageSearchFilter: PageParam
    {
        public int? UserId { get; set; }
        public string? Role { get; set; }

        public bool? isDeleted { get; set; }
    }
}

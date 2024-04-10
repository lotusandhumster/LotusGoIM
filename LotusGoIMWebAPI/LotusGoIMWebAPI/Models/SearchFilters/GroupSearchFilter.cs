namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class GroupSearchFilter : PageParam
    {
        public string? Name { get; set; } = string.Empty;
        public int? Owner { get; set; }
        public int? MemberId { get; set; }
        public bool IsDeleted { get; set; }
    }
}

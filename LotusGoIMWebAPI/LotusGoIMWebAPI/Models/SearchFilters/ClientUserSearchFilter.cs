using LotusGoIMWebAPI.Common.Enums;

namespace LotusGoIMWebAPI.Models.SearchFilters
{
    public class ClientUserSearchFilter:PageParam
    {
        public string? NickName { get; set; } = String.Empty;
        public string? Email { get; set; } = String.Empty;
        public GenderType? Gender { get; set; }
        public ClientUserState? State { get; set; }
        public bool IsDeleted { get; set; }
    }
}

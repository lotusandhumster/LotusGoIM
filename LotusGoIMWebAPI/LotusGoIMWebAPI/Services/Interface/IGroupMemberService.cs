using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IGroupMemberService
    {
        Task<bool> AddAsync(GroupMember groupMember);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(GroupMember groupMember);
        Task<List<GroupMemberModel>> GetListAsync(GroupMemberSearchFilter filter);
        Task<int> GetCountAsync(int groupId);
        Task<GroupMemberModel?> GetAsync(int id);
    }
}

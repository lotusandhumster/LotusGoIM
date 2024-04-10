using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IGroupService
    {
        Task<bool>AddAsync(Group group);
        Task<Group?> GetAsync(int groupId);
        Task<bool> UpdateAsync(Group group);
        Task<bool> DeleteAsync(int groupId);
        Task<IEnumerable<Group>> GetListAsync(GroupSearchFilter filter);
        Task<PageResultModel<IEnumerable<Group>>> GetPageAsync(GroupSearchFilter filter);
    }
}

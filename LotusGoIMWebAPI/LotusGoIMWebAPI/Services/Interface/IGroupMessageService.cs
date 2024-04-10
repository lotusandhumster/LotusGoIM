using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Models;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IGroupMessageService
    {
        Task<bool> AddAsync(GroupMessage groupMessage);
        Task<bool> DeleteAsync(int id);
        Task<GroupMessageModel> GetAsync(int id);
        Task<IEnumerable<GroupMessageModel>> GetListAsync(GroupMessageSearchFilter filter);
        Task<PageResultModel<IEnumerable<GroupMessageModel>>> GetPageAsync(GroupMessageSearchFilter filter);
    }
}

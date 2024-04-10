using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IPrivateMessageService
    {
        Task<bool> AddAsync(PrivateMessage privateMessage);
        Task<bool> DeleteAsync(int id);
        Task<PrivateMessageModel> GetAsync(int id);
        Task<IEnumerable<PrivateMessageModel>> GetListAsync(PrivateMessageSearchFilter filter);
        Task<PageResultModel<IEnumerable<PrivateMessageModel>>> GetPageAsync(PrivateMessageSearchFilter filter);

    }
}

using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IFriendService
    {
        Task<ResultModel<bool>> AddAsync(Friend friend);
        Task<bool> DeleteAsync(int id);
        Task<ResultModel<bool>> UpdateAsync(Friend friend);
        Task<ResultModel<List<FriendModel>>> GetListAsync(FriendSearchFilter filter);
        Task<ResultModel<FriendModel>> GetAsync(int id);
        Task<int> GetCountAsync(int userId);

    }
}

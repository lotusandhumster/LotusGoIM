using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IClientUserService
    {
        Task<ResultModel<bool>> AddAsync(ClientUser user);

        Task<ClientUser?> GetAsync(int id);

        Task<List<ClientUser>> GetListAsync(ClientUserSearchFilter filter);

        Task<List<ClientUser>> GetPageAsync(ClientUserSearchFilter filter);

        Task<ResultModel<bool>> UpdateAsync(ClientUser user);
        Task<bool> DeleteAsync(int id);
        Task<ClientUser?> LoginAsync(LoginModel loginModel);
        Task<int> GetCountAsync(ClientUserSearchFilter filter);
        Task<ResultModel<bool>> UpdatePasswordAsync(int id, string password);
        Task<ClientUser?> GetByEmailAsync(string email);
    }
}
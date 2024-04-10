using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Models;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IChatGptMessageService
    {
        Task<bool> AddAsync(ChatGptMessage message);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ChatGptMessageWithUserModel>> GetListAsync(ChatGptMessageSearchFilter filter);
        Task<PageResultModel<IEnumerable<ChatGptMessageWithUserModel>>> GetPageAsync(ChatGptMessageSearchFilter filter);
        Task<ChatGptMessageWithUserModel> GetAsync(int id);
        Task<int> GetCount(int userId);
        Task<IEnumerable<ChatGptMessageWithUserModel>> GetLast20ListAsync(int userId);
        Task<bool> ClearAllMessageAsync(int userId);
    }
}

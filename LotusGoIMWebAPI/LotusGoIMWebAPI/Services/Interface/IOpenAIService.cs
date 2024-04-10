using LotusGoIMWebAPI.Models;

namespace LotusGoIMWebAPI.Services.Interface
{
    public interface IOpenAIService
    {
        Task<string> ChatAsync(string prompt);
        Task<string> ChatWithHistoryAsync(string prompt, IEnumerable<ChatGptMessageWithUserModel> conversationHistory);
        Task<string> QuickReplyAsync(string prompt);
    }
}

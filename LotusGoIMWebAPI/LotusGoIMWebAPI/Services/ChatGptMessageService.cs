using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.DbContexts;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LotusGoIMWebAPI.Services
{
    public class ChatGptMessageService : IChatGptMessageService
    {
        private readonly LotusGoIMContext _context;
        private readonly IClientUserService _clientUserService;

        public ChatGptMessageService(LotusGoIMContext context, IClientUserService clientUserService)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _clientUserService = clientUserService;
        }

        public async Task<bool> AddAsync(ChatGptMessage message)
        {
            await _context.ChatGptMessage.AddAsync(message);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ClearAllMessageAsync(int userId)
        {
            var messages = await _context.ChatGptMessage.Where(m => m.UserId == userId).ToListAsync();

            foreach (var message in messages)
            {
                message.IsDeleted = true;
                _context.ChatGptMessage.Update(message);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var message = await _context.ChatGptMessage.FirstOrDefaultAsync(m => m.ChatGptMessageId == id);
            if (message == null)
            {
                return false;
            }
            message.IsDeleted = true;
            _context.Update(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ChatGptMessageWithUserModel> GetAsync(int id)
        {
            var message = await _context.ChatGptMessage.FirstOrDefaultAsync(m => m.ChatGptMessageId == id);
            if (message == null)
            {
                return new ChatGptMessageWithUserModel();
            }
            var user = await _clientUserService.GetAsync(message.UserId);

            return new ChatGptMessageWithUserModel(message, user!);
        }

        public async Task<int> GetCount(int userId)
        {
            var query = _context.ChatGptMessage.AsQueryable();

            query = query.Where(u => u.UserId == userId);
            query = query.Where(u => !u.IsDeleted);

            return await query.CountAsync();
        }

        public async Task<IEnumerable<ChatGptMessageWithUserModel>> GetLast20ListAsync(int userId)
        {
            var query = _context.ChatGptMessage.AsQueryable();
            query = query.Where(u => u.UserId == userId && !u.IsDeleted);
            var messageList = await query.OrderByDescending(u => u.SendTime).Take(20).ToListAsync();

            var result = new List<ChatGptMessageWithUserModel>();

            foreach (var message in messageList)
            {
                var user = await _clientUserService.GetAsync(message.UserId);
                result.Add(new ChatGptMessageWithUserModel(message, user!));
            }

            return result;
        }

        public async Task<IEnumerable<ChatGptMessageWithUserModel>> GetListAsync(ChatGptMessageSearchFilter filter)
        {
            var query = _context.ChatGptMessage.AsQueryable();

            if (filter.UserId.HasValue)
            {
                query = query.Where(u => u.UserId == filter.UserId.Value);
            }
            if (filter.Role != null)
            {
                query = query.Where(u => u.Role == filter.Role);
            }
            if (filter.isDeleted != null)
            {
                query = query.Where(u => u.IsDeleted == filter.isDeleted);
            }
            query = query.OrderByDescending(m => m.SendTime);

            var result = new List<ChatGptMessageWithUserModel>();

            var messageList = await query.ToListAsync();

            foreach (var message in messageList)
            {
                var user = await _clientUserService.GetAsync(message.UserId);
                result.Add(new ChatGptMessageWithUserModel(message, user!));
            }

            return result;
        }

        public async Task<PageResultModel<IEnumerable<ChatGptMessageWithUserModel>>> GetPageAsync(ChatGptMessageSearchFilter filter)
        {
            var query = _context.ChatGptMessage.AsQueryable();

            if (filter.UserId.HasValue)
            {
                query = query.Where(u => u.UserId == filter.UserId.Value);
            }
            if (filter.Role != null)
            {
                query = query.Where(u => u.Role == filter.Role);
            }
            if (filter.isDeleted != null)
            {
                query = query.Where(u => u.IsDeleted == filter.isDeleted);
            }
            query = query.OrderByDescending(m => m.SendTime);
            query = query.Skip(filter.PageSize * (filter.PageIndex - 1)).Take(filter.PageSize);

            var total = await query.CountAsync();

            var result = new List<ChatGptMessageWithUserModel>();

            var messageList = await query.ToListAsync();

            foreach (var message in messageList)
            {
                var user = await _clientUserService.GetAsync(message.UserId);
                result.Add(new ChatGptMessageWithUserModel(message, user!));
            }

            return ResultModelFactory.PageResultModelSuccess<IEnumerable<ChatGptMessageWithUserModel>>(result, total);
        }

    }
}

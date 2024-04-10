using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.DbContexts;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LotusGoIMWebAPI.Services
{
    public class GroupMessageService : IGroupMessageService
    {
        private readonly LotusGoIMContext _context;

        public GroupMessageService(LotusGoIMContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public async Task<bool> AddAsync(GroupMessage groupMessage)
        {
            if (!Enum.IsDefined(typeof(MessageType), groupMessage.Type))
            {
                return false;
            }
            if (groupMessage.Content.Length > 1000)
            {
                return false;
            }

            await _context.GroupMessage.AddAsync(groupMessage);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var message = await _context.GroupMessage.FindAsync(id);
            if(message == null)
            {
                return false;
            }
            message.IsDeleted = true;

            _context.GroupMessage.Update(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GroupMessageModel> GetAsync(int id)
        {
            var groupMessage = await _context.GroupMessage.FindAsync(id);
            var user = await _context.ClientUser.FindAsync(groupMessage!.UserId);
            return new GroupMessageModel(groupMessage, user!);
        }

        public async Task<IEnumerable<GroupMessageModel>> GetListAsync(GroupMessageSearchFilter filter)
        {
            var query = _context.GroupMessage.AsQueryable();
            if(filter.GroupId is not null)
            {
                query = query.Where(m => m.GroupId == filter.GroupId);
            }
            if(filter.ClientUserId is not null){
                query = query.Where(m => m.UserId == filter.ClientUserId);
            }
            if (!String.IsNullOrEmpty(filter.Content))
            {
                query = query.Where(m => m.Content.Contains(filter.Content));
            }
            if (filter.Type is not null)
            {
                query = query.Where(m => filter.Type == filter.Type);
            }
            query = query.Where(m => m.IsDeleted == false);
            query = query.OrderByDescending(m => m.SendTime);
            var groupMessageList = await query.ToListAsync();

            var result = new List<GroupMessageModel>();

            foreach(var groupMessage in groupMessageList)
            {
                var user = await _context.ClientUser.FindAsync(groupMessage.UserId);
                var groupMessageModel = new GroupMessageModel(groupMessage, user!);
                result.Add(groupMessageModel);
            }

            return result;
        }

        public async Task<PageResultModel<IEnumerable<GroupMessageModel>>> GetPageAsync(GroupMessageSearchFilter filter)
        {
            var query = _context.GroupMessage.AsQueryable();
            if (filter.GroupId is not null)
            {
                query = query.Where(m => m.GroupId == filter.GroupId);
            }
            if (filter.ClientUserId is not null)
            {
                query = query.Where(m => m.UserId == filter.ClientUserId);
            }
            if (!String.IsNullOrEmpty(filter.Content))
            {
                query = query.Where(m => m.Content.Contains(filter.Content));
            }
            if (filter.Type is not null)
            {
                query = query.Where(m => filter.Type == filter.Type);
            }
            query = query.Where(m => m.IsDeleted == false);

            var total = await query.CountAsync();
            query = query.OrderByDescending(m => m.SendTime);
            query = query.Skip(filter.PageSize * (filter.PageIndex - 1)).Take(filter.PageSize);
            
            var groupMessageList = await query.ToListAsync();

            var result = new List<GroupMessageModel>();

            foreach (var groupMessage in groupMessageList)
            {
                var user = await _context.ClientUser.FindAsync(groupMessage.UserId);
                var groupMessageModel = new GroupMessageModel(groupMessage, user!);
                result.Add(groupMessageModel);
            }

            return ResultModelFactory.PageResultModelSuccess<IEnumerable<GroupMessageModel>>(result, total);
        }
    }
}

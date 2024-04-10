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
    public class PrivateMessageService : IPrivateMessageService
    {
        private readonly LotusGoIMContext _context;

        public PrivateMessageService(LotusGoIMContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public async Task<bool> AddAsync(PrivateMessage privateMessage)
        {
            if (!Enum.IsDefined(typeof(MessageType), privateMessage.Type))
            {
                return false;
            }
            if (privateMessage.Content.Length > 1000)
            {
                return false;
            }

            await _context.PrivateMessage.AddAsync(privateMessage);

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existMessage = await _context.PrivateMessage.FindAsync(id);
            if (existMessage == null)
            {
                return false;
            }

            existMessage.IsDeleted = true;
            _context.PrivateMessage.Update(existMessage);
            return _context.SaveChanges() > 0;
        }

        public async Task<PrivateMessageModel> GetAsync(int id)
        {
            var privateMessage = await _context.PrivateMessage.FindAsync(id);
            var clientUser = await _context.ClientUser.FindAsync(privateMessage!.SenderId);

            return new PrivateMessageModel(privateMessage, clientUser!);
        }

        public async Task<IEnumerable<PrivateMessageModel>> GetListAsync(PrivateMessageSearchFilter filter)
        {
            var query = _context.PrivateMessage.AsQueryable();
            if (filter.SenderId.HasValue && filter.ReceiverId.HasValue)
            {
                query = query.Where(m => m.SenderId == filter.SenderId && m.ReceiverId == filter.ReceiverId
                                        || m.SenderId == filter.ReceiverId && m.ReceiverId == filter.SenderId);
            }
            else
            {
                if (filter.SenderId is not null)
                {
                    query = query.Where(m => m.SenderId == filter.SenderId);
                }
                if (filter.ReceiverId is not null)
                {
                    query = query.Where(m => m.ReceiverId == filter.ReceiverId);
                }
            }
            
            query.Where(m => m.Content.Contains(filter.Content));
            if (filter.Type is not null)
            {
                query = query.Where(m => m.Type == filter.Type);
            }
            query = query.Where(m => m.IsDeleted == false);
            query = query.OrderByDescending(m => m.SendTime);

            var privateMessages = await query.ToListAsync();

            var privateMessageModels = new List<PrivateMessageModel>();
            foreach(var privateMessage in privateMessages)
            {
                var clientUser = await _context.ClientUser.FindAsync(privateMessage.SenderId);
                privateMessageModels.Add(new PrivateMessageModel(privateMessage, clientUser!));
            }

            return privateMessageModels;
        }

        public async Task<PageResultModel<IEnumerable<PrivateMessageModel>>> GetPageAsync(PrivateMessageSearchFilter filter)
        {
            var query = _context.PrivateMessage.AsQueryable();
            if (filter.SenderId.HasValue && filter.ReceiverId.HasValue)
            {
                query = query.Where(m => m.SenderId == filter.SenderId && m.ReceiverId == filter.ReceiverId
                                        || m.SenderId == filter.ReceiverId && m.ReceiverId == filter.SenderId);
            }
            else
            {
                if (filter.SenderId is not null)
                {
                    query = query.Where(m => m.SenderId == filter.SenderId);
                }
                if (filter.ReceiverId is not null)
                {
                    query = query.Where(m => m.ReceiverId == filter.ReceiverId);
                }
            }
            query.Where(m => m.Content.Contains(filter.Content));
            if (filter.Type is not null)
            {
                query = query.Where(m => m.Type == filter.Type);
            }
            query = query.Where(m => m.IsDeleted == false);

            var total = await query.CountAsync();

            query = query.OrderByDescending(m => m.SendTime);
            query = query.Skip(filter.PageSize * (filter.PageIndex - 1)).Take(filter.PageSize);

            var privateMessages = await query.ToListAsync();

            var privateMessageModels = new List<PrivateMessageModel>();
            foreach (var privateMessage in privateMessages)
            {
                var clientUser = await _context.ClientUser.FindAsync(privateMessage.SenderId);
                privateMessageModels.Add(new PrivateMessageModel(privateMessage, clientUser!));
            }

            return ResultModelFactory.PageResultModelSuccess<IEnumerable<PrivateMessageModel>>(privateMessageModels, total);
        }
    }
}

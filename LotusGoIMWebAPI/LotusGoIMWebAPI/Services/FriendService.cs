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
    public class FriendService : IFriendService
    {
        private readonly LotusGoIMContext _context;
        public FriendService(LotusGoIMContext lotusGoIMContext)
        {
            _context = lotusGoIMContext;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public async Task<ResultModel<bool>> AddAsync(Friend friend)
        {
            int userId = friend.UserId;
            int friendUserId = friend.FriendUserId;
            var existFriend = await _context.Friend.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendUserId == friendUserId && !f.IsDeleted);
            if (existFriend != null)
            {
                return ResultModelFactory.ResultModelSusccess<bool>();
            }
            else
            {
                existFriend = await _context.Friend.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendUserId == friendUserId);
                if (existFriend != null)
                {
                    existFriend.IsDeleted = false;
                    _context.Friend.Update(existFriend);
                    return ResultModelFactory.ResultModelSusccess(await _context.SaveChangesAsync() > 0);
                }
            }
            if (friend.UserId == friend.FriendId)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("不能添加自己为好友");
            }
            if (friend.Remark is not null)
            {
                if (friend.Remark.Length > 20)
                {
                    return ResultModelFactory.ResultModelInternalServerError<bool>("备注长度不符合要求");
                }
            }
            else
            {
                friend.Remark = "";
            }
            _ = await _context.Friend.AddAsync(friend);
            return ResultModelFactory.ResultModelSusccess(await _context.SaveChangesAsync() > 0);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var friend = await _context.Friend.FirstOrDefaultAsync(f => f.FriendId == id);

            if (friend is null)
            {
                return false;
            }

            friend.IsDeleted = true;
            _context.Friend.Update(friend);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetCountAsync(int userId)
        {
            var count = await _context.Friend.Where(u => u.UserId == userId && !u.IsDeleted).CountAsync();
            return count;
        }

        public async Task<ResultModel<FriendModel>> GetAsync(int id)
        {
            var friend = await _context.Friend.FindAsync(id);
            if(friend is null)
            {
                return ResultModelFactory.ResultModelInternalServerError<FriendModel>("没有找到该好友");
            }
            var friendUser = await _context.ClientUser.FindAsync(friend.FriendUserId);
            if (friendUser is null)
            {
                return ResultModelFactory.ResultModelInternalServerError<FriendModel>("没有找到该好友的用户信息");
            }
            var friendModel = new FriendModel(friend, friendUser);
            return ResultModelFactory.ResultModelSusccess(friendModel);
        }

        public async Task<ResultModel<List<FriendModel>>> GetListAsync(FriendSearchFilter filter)
        {
            var query = _context.Friend.Where(f => f.UserId == filter.UserId)
                .Where(f => f.IsDeleted == filter.IsDeleted);
            var friendList = await query.Select(f => f).ToListAsync();
            if(friendList is null)
            {
                return ResultModelFactory.ResultModelInternalServerError<List<FriendModel>>("没有找到该好友");
            }
            var friendModelList = new List<FriendModel>();
            foreach (var friend in friendList)
            {
                var friendUser = _context.ClientUser.Find(friend.FriendUserId);
                if (friendUser is null)
                {
                    return ResultModelFactory.ResultModelInternalServerError<List<FriendModel>>("没有找到该好友的用户信息");
                }
                var friendModel = new FriendModel(friend, friendUser);
                friendModelList.Add(friendModel);
            }
            var rQuery = friendModelList.AsQueryable();
            if (!string.IsNullOrEmpty(filter.NickName))
            {
                rQuery = rQuery.Where(f => f.NickName.Contains(filter.NickName));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                rQuery = rQuery.Where(f => f.Email.Contains(filter.Email));
            }
            if (!string.IsNullOrEmpty(filter.Remark))
            {
                rQuery = rQuery.Where(f => (f.Remark != null && f.Remark.Contains(filter.Remark))
                                    || f.NickName.Contains(filter.Remark));
            }
            if (filter.Type.HasValue)
            {
                rQuery = rQuery.Where(f => f.Type == filter.Type);
            }
            if (filter.State.HasValue)
            {
                rQuery = rQuery.Where(f => f.State == filter.State);
            }
            rQuery = rQuery.Where(f => f.IsDeleted == false).OrderBy(f => f.State);
            return ResultModelFactory.ResultModelSusccess(rQuery.ToList());
        }

        public async Task<ResultModel<bool>> UpdateAsync(Friend friend)
        {
            var friendEntity = await _context.Friend.FindAsync(friend.FriendId);
            if (friendEntity is null)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("没有找到该好友");
            }
            if (friend.Remark is not null)
            {
                if(friend.Remark.Length > 20)
                {
                    return ResultModelFactory.ResultModelInternalServerError<bool>("备注长度不符合要求");
                }
            }
            else
            {
                friend.Remark = "";
            }
            if (!Enum.IsDefined(typeof(FriendType), friend.Type))
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("好友类型有误");
            }
            friendEntity.Type = friend.Type;
            friendEntity.Remark = friend.Remark;
            _context.Friend.Update(friendEntity);
            return ResultModelFactory.ResultModelSusccess(_context.SaveChanges() > 0);
        }
    }
}

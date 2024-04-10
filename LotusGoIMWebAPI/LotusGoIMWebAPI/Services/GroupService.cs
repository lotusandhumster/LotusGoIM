using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.DbContexts;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LotusGoIMWebAPI.Services
{
    public class GroupService : IGroupService
    {
        private readonly LotusGoIMContext _dbContext;
        public GroupService(LotusGoIMContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public async Task<bool> AddAsync(Group group)
        {
            var result = await _dbContext.Group.AddAsync(group);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int groupId)
        {
            var group = await _dbContext.Group.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return false;
            }

            group.IsDeleted = true;
            _dbContext.Group.Update(group);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Group?> GetAsync(int groupId)
        {
            return await _dbContext.Group.FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<IEnumerable<Group>> GetListAsync(GroupSearchFilter filter)
        {
            var query = _dbContext.Group.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(g => g.Name.Contains(filter.Name));
            }
            if (filter.Owner.HasValue)
            {
                query = query.Where(g => g.Owner == filter.Owner);
            }
            if (filter.MemberId.HasValue)
            {
                var memberQuery = _dbContext.GroupMember.Where(gm => gm.MemberId == filter.MemberId
                                    && !gm.IsDeleted);
                var groupIds = memberQuery.Select(gm => gm.GroupId);
                query = query.Where(g => groupIds.Contains(g.GroupId));
            }
            query = query.Where(g => g.IsDeleted == filter.IsDeleted);
            return await query.ToListAsync();
        }

        public async Task<PageResultModel<IEnumerable<Group>>> GetPageAsync(GroupSearchFilter filter)
        {
            var query = _dbContext.Group.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(g => g.Name.Contains(filter.Name));
            }
            if (filter.Owner.HasValue)
            {
                query = query.Where(g => g.Owner == filter.Owner);
            }
            if (filter.MemberId.HasValue)
            {
                var memberQuery = _dbContext.GroupMember.Where(gm => gm.MemberId == filter.MemberId);
                var groupIds = memberQuery.Select(gm => gm.GroupId);
                query = query.Where(g => groupIds.Contains(g.GroupId));
            }
            query = query.Where(g => g.IsDeleted == filter.IsDeleted);

            var total = await query.CountAsync();
            var data = await  query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            return ResultModelFactory.PageResultModelSuccess<IEnumerable<Group>>(data, total);
        }

        public async Task<bool> UpdateAsync(Group group)
        {
            var groupEntity = await _dbContext.Group.FirstOrDefaultAsync(g => g.GroupId == group.GroupId);

            if (groupEntity == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(group.Name))
            {
                groupEntity.Name = group.Name;
            }
            if (!string.IsNullOrEmpty(group.AvatarUrl))
            {
                groupEntity.AvatarUrl = group.AvatarUrl;
            }
            if (!string.IsNullOrEmpty(group.Description))
            {
                groupEntity.Description = group.Description;
            }
            if (group.Owner != 0)
            {
                groupEntity.Owner = group.Owner;
            }

            _dbContext.Group.Update(groupEntity);

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

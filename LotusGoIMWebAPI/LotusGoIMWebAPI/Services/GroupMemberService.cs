using LotusGoIMWebAPI.DbContexts;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LotusGoIMWebAPI.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly LotusGoIMContext _context;

        public GroupMemberService(LotusGoIMContext lotusGoIMContext)
        {
            _context = lotusGoIMContext;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<bool> AddAsync(GroupMember groupMember)
        {
            if (groupMember.MemberName.Length > 20)
            {
                return false;
            } 

            var existGroupMember = await _context.GroupMember.FirstOrDefaultAsync(gm => gm.GroupId == groupMember.GroupId && gm.MemberId == groupMember.MemberId && !gm.IsDeleted);
            if (existGroupMember is not null)
            {
                return false;
            }

            _ = await _context.GroupMember.AddAsync(groupMember);
            return await _context.SaveChangesAsync() > 0;            
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var groupMember = await _context.GroupMember.FirstOrDefaultAsync(gm => gm.GroupMemberId == id);

            if (groupMember is null)
            {
                return false;
            }

            groupMember.IsDeleted = true;
            _context.GroupMember.Update(groupMember);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GroupMemberModel?> GetAsync(int id)
        {
            var groupMember = await _context.GroupMember.FirstOrDefaultAsync(gm => gm.GroupMemberId == id);
            if (groupMember is null)
            {
                return null;
            }

            var clientUser = await _context.ClientUser.Where(cu => cu.UserId == groupMember.MemberId).FirstOrDefaultAsync();
            if (clientUser is null)
            {
                return null;
            }

            return new GroupMemberModel(groupMember, clientUser);
        }

        public async Task<int> GetCountAsync(int groupId)
        {
            return await _context.GroupMember.Where(gm => gm.GroupId == groupId && !gm.IsDeleted).CountAsync();
        }

        public async Task<List<GroupMemberModel>> GetListAsync(GroupMemberSearchFilter filter)
        {
            var query = _context.GroupMember.AsQueryable();
            if (filter.GroupId.HasValue)
            {
                query = query.Where(gm => gm.GroupId == filter.GroupId);
            }
            if (!String.IsNullOrEmpty(filter.MemberName))
            {
                query = query.Where(gm => gm.MemberName.Contains(filter.MemberName));
            }
            if (filter.MemberId.HasValue)
            {
                query = query.Where(gm => gm.MemberId == filter.MemberId);
            }
            query = query.Where(gm => gm.IsDeleted == false);
            var groupMembers = await query.ToListAsync();
            
            var groupMemberModels = new List<GroupMemberModel>();
            foreach (var groupMember in groupMembers)
            {
                var clientUser = await _context.ClientUser.Where(cu => cu.UserId == groupMember.MemberId).FirstOrDefaultAsync();
                groupMemberModels.Add(new GroupMemberModel(groupMember, clientUser!));
            }

            return groupMemberModels;
        }

        public async Task<bool> UpdateAsync(GroupMember groupMember)
        {
            var existGroupMember = _context.GroupMember.FirstOrDefault(gm => gm.GroupMemberId == groupMember.GroupMemberId);

            if (existGroupMember is null)
            {
                return false;
            }

            if(groupMember.MemberName.Length > 20)
            {
                return false;
            } 

            existGroupMember.MemberName = groupMember.MemberName;

            _context.GroupMember.Update(existGroupMember);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}

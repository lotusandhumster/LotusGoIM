using LotusGoIMWebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LotusGoIMWebAPI.DbContexts
{
    public class LotusGoIMContext : DbContext
    {
        public DbSet<ClientUser> ClientUser { get; set; }
        public DbSet<Friend> Friend { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<GroupMessage> GroupMessage { get; set; }
        public DbSet<PrivateMessage> PrivateMessage { get; set; }
        public DbSet<ChatGptMessage> ChatGptMessage { get; set; }

        public LotusGoIMContext(DbContextOptions<LotusGoIMContext> options) : base(options)
        {
        }
        public LotusGoIMContext()
        {
            
        }
    }
}

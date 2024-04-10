using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Models
{
    public class GroupMemberModel : GroupMember
    {
        public int UserId { get; set; }
        public string NickName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string? AvatarUrl { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Description { get; set; } = String.Empty;
        public ClientUserState State { get; set; } = ClientUserState.Offline;

        public GroupMemberModel(GroupMember groupMember, ClientUser clientUser)
        {
            GroupMemberId = groupMember.GroupMemberId;
            GroupId = groupMember.GroupId;
            MemberId = groupMember.MemberId;
            IsDeleted = groupMember.IsDeleted;
            MemberName = groupMember.MemberName;
            UserId = clientUser.UserId;
            AvatarUrl = clientUser.AvatarUrl;
            Gender = clientUser.Gender;
            Description = clientUser.Description;
            State = clientUser.State;
            Email = clientUser.Email;
            NickName = clientUser.NickName;
        }
    }
}

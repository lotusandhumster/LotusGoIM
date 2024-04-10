using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace LotusGoIMWebAPI.Models
{
    public class FriendModel : Friend
    {
        public string NickName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string? AvatarUrl { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Description { get; set; } = String.Empty;

        public ClientUserState State { get; set; } = ClientUserState.Offline;

        public FriendModel(Friend friend, ClientUser clientUser)
        {
            FriendId = friend.FriendId;
            UserId = friend.UserId;
            FriendUserId = friend.FriendUserId;
            NickName = clientUser.NickName;
            Email = clientUser.Email;
            AvatarUrl = clientUser.AvatarUrl;
            Gender = clientUser.Gender;
            Remark = friend.Remark;
            State = clientUser.State;
            Type = friend.Type;
            Description = clientUser.Description;
        }

        public FriendModel()
        {
        }
    }
}

using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;

namespace LotusGoIMWebAPI.Models
{
    public class GroupMessageModel:GroupMessage
    {
        public string NickName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string? AvatarUrl { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Description { get; set; } = String.Empty;

        public GroupMessageModel(GroupMessage groupMessage, ClientUser clientUser)
        {
            GroupMessageId = groupMessage.GroupMessageId;
            UserId = groupMessage.UserId;
            GroupId = groupMessage.GroupId;
            Content = groupMessage.Content;
            SendTime = groupMessage.SendTime;
            NickName = clientUser.NickName;
            Email = clientUser.Email;
            AvatarUrl = clientUser.AvatarUrl;
            Gender = clientUser.Gender;
            Description = clientUser.Description;
            Type = groupMessage.Type;
            FileUrl = groupMessage.FileUrl;
        }
    }
}

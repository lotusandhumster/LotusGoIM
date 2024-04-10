using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;

namespace LotusGoIMWebAPI.Models
{
    public class PrivateMessageModel: PrivateMessage
    {
        public string NickName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string? AvatarUrl { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Description { get; set; } = String.Empty;

        public PrivateMessageModel(PrivateMessage privateMessage, ClientUser clientUser)
        {
            PrivateMessageId = privateMessage.PrivateMessageId;
            SenderId = privateMessage.SenderId;
            ReceiverId = privateMessage.ReceiverId;
            Content = privateMessage.Content;
            SendTime = privateMessage.SendTime;
            Type = privateMessage.Type;
            FileUrl = privateMessage.FileUrl;
            NickName = clientUser.NickName;
            Email = clientUser.Email;
            AvatarUrl = clientUser.AvatarUrl;
            Gender = clientUser.Gender;
            Description = clientUser.Description;
        }
    }
}

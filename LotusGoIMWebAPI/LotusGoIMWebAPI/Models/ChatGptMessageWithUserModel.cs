using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;

namespace LotusGoIMWebAPI.Models
{
    public class ChatGptMessageWithUserModel: ChatGptMessage
    {
        public string NickName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string? AvatarUrl { get; set; }
        public GenderType Gender { get; set; } = GenderType.Other;
        public string Description { get; set; } = String.Empty;

        public ChatGptMessageWithUserModel(ChatGptMessage message, ClientUser user)
        {
            NickName = user.NickName;
            Email = user.Email;
            AvatarUrl = user.AvatarUrl;
            Gender = user.Gender;
            Description = user.Description;
            ChatGptMessageId = message.ChatGptMessageId;
            UserId = message.UserId;
            Role = message.Role;
            Content = message.Content;
            IsDeleted = message.IsDeleted;
            SendTime = message.SendTime;
        }

        public ChatGptMessageWithUserModel()
        {
            
        }
    }
}

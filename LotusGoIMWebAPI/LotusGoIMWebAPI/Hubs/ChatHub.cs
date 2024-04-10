using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LotusGoIMWebAPI.Hubs
{

    [Authorize(Roles = "user")]
    public class ChatHub: Hub
    {
        private readonly IPrivateMessageService _privateMessageService;
        private readonly IClientUserService _clientUserService;
        private readonly IGroupMessageService _groupMessageService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;

        public ChatHub(IPrivateMessageService privateMessageService,
            IClientUserService clientUserService,
            IGroupMessageService groupMessageService,
            IGroupService groupService,
            IGroupMemberService groupMemberService)
        {
            _privateMessageService = privateMessageService;
            _clientUserService = clientUserService;
            _groupMessageService = groupMessageService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var clientUser = await _clientUserService.GetAsync(Convert.ToInt32(userId));
                if (clientUser != null)
                {
                    clientUser.State = ClientUserState.Online;
                    await _clientUserService.UpdateAsync(clientUser);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var clientUser = await _clientUserService.GetAsync(Convert.ToInt32(userId));
                if (clientUser != null)
                {
                    clientUser.State = ClientUserState.Offline;
                    await _clientUserService.UpdateAsync(clientUser);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendPrivateMessage(PrivateMessage message)
        {
            message.SendTime = DateTime.Now;

            await _privateMessageService.AddAsync(message);

            await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceivePrivateMessage", message);
        }

        public async Task SendGroupMessage(GroupMessage message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                message.UserId = Convert.ToInt32(userId);
                message.SendTime = DateTime.Now;

                await _groupMessageService.AddAsync(message);

                await Clients.Group(message.GroupId.ToString()).SendAsync("ReceiveGroupMessage", message);
            }
        }

        public async Task JoinGroup(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task LeaveGroup(int groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task QuitGroup(int groupId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await LeaveGroup(groupId);

            var filter = new GroupMemberSearchFilter();
            filter.GroupId = groupId;
            filter.MemberId = Convert.ToInt32(userId);

            var member = (await _groupMemberService.GetListAsync(filter)).FirstOrDefault();

            if(member != null)
            {
                await _groupMemberService.DeleteAsync(member.GroupMemberId);
                await Clients.Group(member.GroupId.ToString()).SendAsync("ReceiveQuitGroup", userId);
            }
        }

        public async Task RemoveGroup(int groupId)
        {
            await _groupService.DeleteAsync(groupId);

            await Clients.Group(groupId.ToString()).SendAsync("ReceiveRemoveGroup", groupId);
        }

        public async Task InviteGroupMember(int groupId, int userId)
        {
            var member = new GroupMember();
            member.GroupId = groupId;
            member.MemberId = userId;

            await _groupMemberService.AddAsync(member);

            await Clients.Group(groupId.ToString()).SendAsync("ReceiveInviteGroupMember", member);
            await Clients.User(userId.ToString()).SendAsync("ReceiveInviteGroupMember", member);
        }

    }
}

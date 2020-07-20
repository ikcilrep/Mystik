using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mystik.Entities;
using Mystik.Helpers;
using Mystik.Services;

namespace Mystik.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private IMessageService _messageService;
        private IConversationService _conversationService;
        private IUserService _userService;

        public ChatHub(IMessageService messageService,
                       IConversationService conversationService,
                       IUserService userService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _userService = userService;
        }

        public async Task SendMessage(byte[] encryptedContent, Guid conversationId)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            var conversation = await _conversationService.Retrieve(conversationId);
            if (conversation != null
                && await _messageService.IsTheConversationMember(conversationId, currentUserId))
            {
                var message = await _messageService.Create(encryptedContent, currentUserId, conversationId);
                var sender = await _userService.Retrieve(currentUserId);
                await Clients.Users(conversation.Members)
                             .ReceiveMessage(message.Id, encryptedContent, message.SentTime, sender.Nickname);
            }
        }

        public async Task CreateConversation(string name, byte[] passwordHashData, IEnumerable<Guid> usersIds)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            var conversation = await _conversationService.Create(name, passwordHashData, currentUserId);
            var membersIds = usersIds.ToHashSet();

            membersIds.Add(currentUserId);

            await _conversationService.AddMembers(conversation.Id, membersIds);

            await Clients.Users(membersIds.ToStringList()).CreateConversation(conversation.Id);
        }

        public async Task DeleteConversation(Guid conversationId)
        {
            if (await CanTheCurrentUserModifyTheConversation(conversationId))
            {
                var members = await _conversationService.Delete(conversationId);
                await Clients.Users(members).DeleteConversation(conversationId);
            }
        }

        public async Task ChangeConversationName(Guid conversationId, string newName)
        {
            if (await CanTheCurrentUserModifyTheConversation(conversationId))
            {
                var members = await _conversationService.ChangeName(conversationId, newName);
                await Clients.Users(members).ChangeConversationName(conversationId, newName);
            }
        }

        private async Task<bool> CanTheCurrentUserModifyTheConversation(Guid conversationId)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            return Context.User.IsInRole(Role.Admin)
                   || await _conversationService.IsTheConversationManager(conversationId, currentUserId);
        }

        public async Task InviteFriends(List<Guid> invitedIds)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            await _userService.InviteFriends(currentUserId, invitedIds);

            await Clients.Users(invitedIds.ToStringList()).ReceiveInvitation(currentUserId);
        }

        public async Task DeleteInvitations(List<Guid> invitedIds)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            await _userService.DeleteInvitations(currentUserId, invitedIds);

            await Clients.Users(invitedIds.ToStringList()).DeleteInvitation(currentUserId);
        }

        public async Task AddFriend(Guid inviterId)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            if (await _userService.IsUserInvited(inviterId, currentUserId))
            {
                await _userService.AddFriend(inviterId, currentUserId);

                await Clients.User(inviterId.ToString()).AddFriend(inviterId);
            }
        }

        public async Task DeleteFriends(List<Guid> friendsIds)
        {
            var currentUserId = Guid.Parse(Context.User.Identity.Name);
            await _userService.DeleteFriends(currentUserId, friendsIds);

            await Clients.Users(friendsIds.ToStringList()).DeleteFriend(currentUserId);
        }
    }
}
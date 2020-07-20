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

            await _conversationService.AddUsers(conversation.Id, membersIds);

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

        public async Task InviteFriends(Guid inviterId, List<Guid> invitedIds)
        {
            await _userService.InviteFriends(inviterId, invitedIds);

            await Clients.Users(invitedIds.ToStringList()).ReceiveInvitation(inviterId);
        }

        public async Task DeleteInvitations(Guid inviterId, List<Guid> invitedIds)
        {
            await _userService.DeleteInvitations(inviterId, invitedIds);

            await Clients.Users(invitedIds.ToStringList()).DeleteInvitation(inviterId);
        }
    }
}
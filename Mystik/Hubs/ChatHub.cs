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
            var currentUserId = Context.GetCurrentUserId();
            var conversation = await _conversationService.Retrieve(conversationId);
            if (conversation != null
                && await _messageService.IsTheConversationMember(conversationId, currentUserId))
            {
                var message = await _messageService.Create(encryptedContent, currentUserId, conversationId);
                var messageToSend = new JsonRepresentableMessage
                {
                    Id = message.Id,
                    EncryptedContent = encryptedContent,
                    CreatedDate = message.CreatedDate,
                    Sender = new UserPublicData { Id = currentUserId }
                };

                await Clients.Users(conversation.GetMembers())
                             .ReceiveMessage(messageToSend);
            }
        }

        public async Task EditMessage(Guid messageId, byte[] newEncryptedContent)
        {
            var currentUserId = Context.GetCurrentUserId();
            var message = await _messageService.Retrieve(messageId);

            if (message != null && message.SenderId == currentUserId)
            {
                await _messageService.Edit(messageId, newEncryptedContent);

                var conversation = await _conversationService.Retrieve(message.ConversationId);

                await Clients.Users(conversation.GetMembers()).EditMessage(messageId, newEncryptedContent);
            }
        }

        public async Task DeleteMessage(Guid messageId)
        {
            var currentUserId = Context.GetCurrentUserId();
            var message = await _messageService.Retrieve(messageId);

            if (message != null && message.SenderId == currentUserId)
            {
                await _messageService.Delete(messageId);

                var conversation = await _conversationService.Retrieve(message.ConversationId);

                await Clients.Users(conversation.GetMembers()).DeleteMessage(messageId);
            }
        }

        public async Task CreateConversation(string name, byte[] passwordHashData, IEnumerable<Guid> usersIds)
        {
            var currentUserId = Context.GetCurrentUserId();
            var conversation = await _conversationService.Create(name, passwordHashData, currentUserId);
            var membersIds = usersIds.ToHashSet();

            membersIds.Add(currentUserId);

            await _conversationService.AddMembers(conversation.Id, membersIds);

            var representableConversation = await conversation.ToJsonRepresentableObject();

            await Clients.Users(membersIds.ToStringList()).JoinConversation(representableConversation);
        }

        public async Task DeleteConversation(Guid conversationId)
        {
            if (await CanTheCurrentUserModifyTheConversation(conversationId))
            {
                var members = await _conversationService.Delete(conversationId);

                members = members.Where(id => id != Context.GetCurrentUserId().ToString()).ToStringList();

                await Clients.Users(members).LeaveConversation(conversationId);
            }
        }

        public async Task ChangeConversationName(Guid conversationId, string newName)
        {
            if (await CanTheCurrentUserModifyTheConversation(conversationId))
            {
                var members = await _conversationService.ChangeName(conversationId, newName);

                members = members.Where(id => id != Context.GetCurrentUserId().ToString()).ToStringList();

                await Clients.Users(members).ChangeConversationName(conversationId, newName);
            }
        }

        private async Task<bool> CanTheCurrentUserModifyTheConversation(Guid conversationId)
        {
            var currentUserId = Context.GetCurrentUserId();
            return await _conversationService.IsTheConversationManager(conversationId, currentUserId);
        }

        public async Task InviteFriends(List<Guid> invitedIds)
        {
            var currentUserId = Context.GetCurrentUserId();
            var usersToNotify = await _userService.InviteFriends(currentUserId, invitedIds);

            await Clients.Users(usersToNotify).ReceiveInvitation(currentUserId);
        }

        public async Task DeleteInvitations(List<Guid> invitedIds)
        {
            var currentUserId = Context.GetCurrentUserId();
            await _userService.DeleteInvitations(currentUserId, invitedIds);

            await Clients.Users(invitedIds.ToStringList()).DeleteInvitation(currentUserId);
        }

        public async Task AddFriend(Guid inviterId)
        {
            var currentUserId = Context.GetCurrentUserId();
            if (await _userService.IsUserInvited(inviterId, currentUserId))
            {
                await _userService.AddFriend(inviterId, currentUserId);

                await Clients.User(inviterId.ToString()).AddFriend(currentUserId);
            }
        }

        public async Task DeleteFriends(List<Guid> friendsIds)
        {
            var currentUserId = Context.GetCurrentUserId();
            await _userService.DeleteFriends(currentUserId, friendsIds);

            await Clients.Users(friendsIds.ToStringList()).DeleteFriend(currentUserId);
        }

        public async Task AddConversationMembers(Guid conversationId, List<Guid> usersIds)
        {
            if (await CanTheCurrentUserModifyTheConversation(conversationId))
            {
                var newMembers = await _conversationService.AddMembers(conversationId, usersIds);

                var conversation = await _conversationService.Retrieve(conversationId);
                var representableConversation = await conversation.ToJsonRepresentableObject();

                await Clients.Users(usersIds.ToStringList()).JoinConversation(representableConversation);
                await Clients.Users(conversation.GetMembers()).AddConversationMembers(conversationId, newMembers);
            }
        }

        public async Task DeleteConversationMembers(Guid conversationId, List<Guid> usersIds)
        {
            var currentUserId = Context.GetCurrentUserId();
            if (await CanTheCurrentUserModifyTheConversation(conversationId) || (usersIds.Count == 1 && usersIds.Single() == currentUserId))
            {
                var usersToDeleteIds = await usersIds.GetUsersToDelete(
                    conversationId,
                    currentUserId,
                    _conversationService);

                var deletedMembers = await _conversationService.DeleteMembers(conversationId, usersToDeleteIds);
                var conversation = await _conversationService.Retrieve(conversationId);

                await Clients.Users(usersToDeleteIds.ToStringList()).LeaveConversation(conversationId);
                await Clients.Users(conversation.GetMembers()).DeleteConversationMembers(conversationId, deletedMembers);
            }
        }

        public async Task UpdateUser(string newNickname, string newPassword)
        {
            var currentUserId = Context.GetCurrentUserId();

            var usersToNotify = await _userService.Update(currentUserId, newNickname, newPassword);

            if (usersToNotify.Any())
            {
                await Clients.Users(usersToNotify).UpdateFriend(currentUserId, newNickname);
            }
        }

        public async Task DeleteUser(Guid userId)
        {
            var currentUserId = Context.GetCurrentUserId();
            if (userId == currentUserId || Context.User.IsInRole(Role.Admin))
            {
                var usersToNotify = await _userService.Delete(userId);

                await Clients.Users(usersToNotify).DeleteFriend(userId);
            }
        }
    }
}
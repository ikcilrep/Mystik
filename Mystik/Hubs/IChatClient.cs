using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;

namespace Mystik.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(JsonRepresentableMessage message);
        Task EditMessage(Guid messageId, byte[] newEncryptedContent);
        Task DeleteMessage(Guid messageId);
        Task JoinConversation(JsonRepresentableConversation conversation);
        Task AddConversationMembers(Guid conversationId, IEnumerable<Guid> membersIds);
        Task LeaveConversation(Guid conversationId);
        Task ChangeConversationName(Guid conversationId, string newName);
        Task ReceiveInvitation(Guid inviterId);
        Task DeleteInvitation(Guid inviterId);
        Task AddFriend(Guid inviterId);
        Task DeleteFriend(Guid friendId);
        Task UpdateFriend(Guid friendId, string newNickname);
    }
}
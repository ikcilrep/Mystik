using System;
using System.Threading.Tasks;

namespace Mystik.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(Guid messageId, byte[] encryptedContent, DateTime sentTime, string senderNickname);
        Task CreateConversation(Guid conversationId);
        Task DeleteConversation(Guid conversationId);
        Task ChangeConversationName(Guid conversationId, string newName);
        Task ReceiveInvitation(Guid inviterId);
        Task DeleteInvitation(Guid inviterId);
        Task AddFriend(Guid inviterId);
        Task DeleteFriend(Guid userId);
    }
}
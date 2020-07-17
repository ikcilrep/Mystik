using System;
using System.Threading.Tasks;

namespace Mystik.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(byte[] encryptedContent, DateTime sentTime, string senderNickname);
        Task ReceiveConversation(Guid conversationId);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;
using Mystik.Models.Message;

namespace Mystik.Services
{
    public interface IMessageService : IDisposable
    {
        IEnumerable<Message> GetMessagesFromConversation(Guid conversationId);
        Task<bool> IsTheConversationMember(Guid conversationId, Guid userId);
        Task<Message> Create(byte[] encryptedContent, Guid senderId, Guid conversationId);
        Task<Message> Retrieve(Guid id);
        Task Edit(Guid id, byte[] newEncryptedContent);
        Task Delete(Guid id);
        Task<IEnumerable<Message>> GetAll();
    }
}

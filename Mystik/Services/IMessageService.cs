using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;

namespace Mystik.Services
{
    public interface IMessageService : IDisposable
    {
        IEnumerable<Message> GetMessagesFromConversation(Guid conversationId);
        Task<bool> IsTheConversationMember(Guid conversationId, Guid userId);
        Task<Message> Create(byte[] encryptedContent, Guid senderId, Guid conversationId);
        Task<Message> Retrieve(Guid id);
        Task Update(Guid id, MessagePatch model);
        Task Delete(Guid id);
        Task<IEnumerable<Message>> GetAll();
    }
}

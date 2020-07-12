using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;

namespace Mystik.Services
{
    public interface IConversationService : IDisposable
    {
        Task<bool> IsTheConversationAdmin(Guid conversationId, Guid userId);
        Task AddUsers(Guid id, HashSet<Guid> users);
        Task<Conversation> Create(string name, byte[] passwordHashData, Guid userId);
        Task<Conversation> Retrieve(Guid id);
        Task Update(Guid id, ConversationPatch model);
        Task Delete(Guid id);
        Task<IEnumerable<Conversation>> GetAll();
    }
}

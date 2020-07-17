using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models.Conversation;

namespace Mystik.Services
{
    public interface IConversationService : IDisposable
    {
        Task<bool> IsTheConversationManager(Guid conversationId, Guid userId);
        Task AddUsers(Guid id, HashSet<Guid> users);
        Task<Conversation> Create(string name, byte[] passwordHashData, Guid userId);
        Task<Conversation> Retrieve(Guid id);
        Task<bool> Update(Guid id, Patch model);
        Task<IReadOnlyList<string>> Delete(Guid id);
        Task<IEnumerable<Conversation>> GetAll();
    }
}

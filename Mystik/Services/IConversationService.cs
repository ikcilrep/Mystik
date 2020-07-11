using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;

namespace Mystik.Services
{
    public interface IConversationService
    {
        Task<Conversation> Create(string name, Guid userId);
        Task<Conversation> Retrieve(Guid id);
        Task Update(Guid id, ConversationPatch model);
        Task Delete(Guid id);
        Task<IEnumerable<Conversation>> GetAll();

        void Dispose();
    }
}

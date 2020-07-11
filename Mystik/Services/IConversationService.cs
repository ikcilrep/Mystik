using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;

namespace Mystik.Services
{
    public interface IConversationService
    {
        Task<User> Create(string name, User user);
        Task<User> Retrieve(Guid id);
        Task Update(Guid id, ConversationPatch model);
        Task Delete(Guid id);
        Task<IEnumerable<User>> GetAll();

        void Dispose();
    }
}

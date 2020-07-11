using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;

namespace Mystik.Services
{
    public class ConversationService : IConversationService
    {
        public Task<User> Create(string name, User user)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<User> Retrieve(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, ConversationPatch model)
        {
            throw new NotImplementedException();
        }
    }
}
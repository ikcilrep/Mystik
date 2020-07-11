using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Models;

namespace Mystik.Services
{
    public class ConversationService : IConversationService
    {
        private DataContext _context;

        public ConversationService(DataContext context)
        {
            _context = context;
        }

        public async Task<Conversation> Create(string name, Guid userId)
        {

            var conversationId = Guid.NewGuid();
            var managedConversation = new ManagedConversation { AdminId = userId, ConversationId = conversationId };
            var conversation = new Conversation
            {
                Id = conversationId,
                Name = name,
                ManagedConversations = new HashSet<ManagedConversation> { managedConversation }
            };
            _context.Add(conversation);
            _context.Add(managedConversation);

            await _context.SaveChangesAsync();
            return conversation;
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Conversation>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Conversation> Retrieve(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, ConversationPatch model)
        {
            throw new NotImplementedException();
        }
    }
}
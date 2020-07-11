using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task Delete(Guid id)
        {
            var conversation = await _context.FindAsync<Conversation>(id);
            var managedConversations = _context.ManagedConversations.Where(mc => mc.ConversationId == id);
            var userConversations = _context.UserConversations.Where(mc => mc.ConversationId == id);
            var messages = _context.Messages.Include(m => m.Conversation).Where(m => m.Conversation.Id == id);

            _context.Remove(conversation);
            _context.Remove(managedConversations);
            _context.Remove(userConversations);
            _context.Remove(messages);

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
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
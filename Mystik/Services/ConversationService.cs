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

            _context.Remove(conversation);
            _context.Remove(managedConversations);
            _context.Remove(userConversations);

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<IEnumerable<Conversation>> GetAll()
        {
            return await _context.Conversations.ToListAsync();
        }

        public async Task<Conversation> Retrieve(Guid id)
        {
            return await _context.FindAsync<Conversation>(id);
        }

        public async Task Update(Guid id, ConversationPatch model)
        {
            var conversation = await _context.FindAsync<Conversation>(id);
            var updatedConversation = model.ToConversation(conversation);

            _context.Entry(conversation).CurrentValues.SetValues(updatedConversation);

            await _context.SaveChangesAsync();
        }
    }
}
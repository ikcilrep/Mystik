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

        public async Task AddUsers(Guid id, HashSet<Guid> usersIds)
        {
            if (usersIds.All(userId => _context.Users.Any(user => user.Id == userId)))
            {
                var userConversations = usersIds.Select(userId => new UserConversation
                {
                    ConversationId = id,
                    UserId = userId
                });

                _context.UserConversations.AddRange(userConversations);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Conversation> Create(string name, byte[] passwordHashData, Guid userId)
        {

            var conversationId = Guid.NewGuid();
            var managedConversation = new ManagedConversation { AdminId = userId, ConversationId = conversationId };
            var conversation = new Conversation
            {
                Id = conversationId,
                Name = name,
                PasswordHashData = passwordHashData,
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

            _context.Remove(conversation);

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<IEnumerable<Conversation>> GetAll()
        {
            return await _context.Conversations.AsNoTracking()
                                               .Include(c => c.Messages)
                                               .Include(c => c.ManagedConversations)
                                               .Include(c => c.UserConversations)
                                               .ToListAsync();
        }

        public async Task<bool> IsTheConversationAdmin(Guid conversationId, Guid userId)
        {
            return await _context.ManagedConversations.AnyAsync(mc => mc.ConversationId == conversationId && mc.AdminId == userId);
        }

        public async Task<Conversation> Retrieve(Guid id)
        {
            return await _context.Conversations.AsNoTracking()
                                               .Include(c => c.Messages)
                                               .Include(c => c.ManagedConversations)
                                               .Include(c => c.UserConversations)
                                               .FirstAsync(c => c.Id == id);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Helpers;

namespace Mystik.Services
{
    public class ConversationService : IConversationService
    {
        private DataContext _context;

        public ConversationService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Guid>> AddMembers(Guid id, IEnumerable<Guid> usersIds)
        {
            var existingUsers = usersIds.Where(userId => _context.Users.Any(user => user.Id == userId));
            if (existingUsers.Any())
            {
                var userConversations = existingUsers.Select(userId => new ConversationMember
                {
                    ConversationId = id,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                });

                _context.ConversationMembers.AddRange(userConversations);
                await _context.SaveChangesAsync();
            }
            return existingUsers.ToList();
        }

        public async Task<Conversation> Create(string name, byte[] passwordHashData, Guid userId)
        {

            var conversationId = Guid.NewGuid();
            var managedConversation = new ConversationManager
            {
                ManagerId = userId,
                ConversationId = conversationId,
                CreatedDate = DateTime.UtcNow
            };
            var conversation = new Conversation
            {
                Id = conversationId,
                Name = name,
                PasswordHashData = passwordHashData,
                Managers = new HashSet<ConversationManager> { managedConversation },
                ModifiedDate = DateTime.UtcNow,
                Messages = new HashSet<Message>{},
                Members = new HashSet<ConversationMember> {}
            };
            _context.Add(conversation);
            _context.Add(managedConversation);

            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<IReadOnlyList<string>> Delete(Guid id)
        {
            var conversation = await _context.Conversations.Include(c => c.Members)
                                                           .FirstOrDefaultAsync(c => c.Id == id);

            _context.Remove(conversation);

            await _context.SaveChangesAsync();
            return conversation.GetMembers();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<IEnumerable<Conversation>> GetAll()
        {
            return await _context.Conversations.AsNoTracking()
                                               .Include(c => c.Messages)
                                               .Include(c => c.Managers)
                                               .Include(c => c.Members)
                                               .ToListAsync();
        }

        public async Task<bool> IsTheConversationManager(Guid conversationId, Guid userId)
        {
            return await _context.ConversationManagers.AnyAsync(cm => cm.ConversationId == conversationId && cm.ManagerId == userId);
        }

        public async Task<Conversation> Retrieve(Guid id)
        {
            return await _context.Conversations.AsNoTracking()
                                               .Include(c => c.Messages)
                                                   .ThenInclude(m => m.Sender)
                                               .Include(c => c.Managers)
                                               .Include(c => c.Members)
                                               .FirstAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<string>> ChangeName(Guid id, string newName)
        {
            var conversation = await _context.Conversations.Include(c => c.Members)
                                                           .FirstOrDefaultAsync(c => c.Id == id);
            if (conversation == null || conversation.Name == newName)
            {
                return new List<string>();
            }

            conversation.Name = newName;
            conversation.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return conversation.GetMembers();
        }

        public async Task<IEnumerable<Guid>> DeleteMembers(Guid conversationId, List<Guid> usersIds)
        {
            var existingMembers = await _context.ConversationMembers.Where(cm => cm.ConversationId == conversationId
                                                                         && usersIds.Contains(cm.UserId)).ToListAsync();

            _context.RemoveRange(existingMembers);

            await _context.SaveChangesAsync();

            return existingMembers.Select(cm => cm.UserId);
        }

        public async Task<IEnumerable<Guid>> GetNotManagingMembersIds(Guid conversationId)
        {
            var conversation = await _context.Conversations.Include(c => c.Members)
                                                           .Include(c => c.Managers)
                                                           .FirstAsync(c => c.Id == conversationId);

            return conversation.Members.Select(cm => cm.UserId)
                                                 .Where(userId => conversation.Managers.All(cm => cm.ManagerId != userId));
        }

    }
}
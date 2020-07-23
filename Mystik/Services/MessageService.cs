using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mystik.Data;
using Mystik.Entities;

namespace Mystik.Services
{
    public class MessageService : IMessageService
    {
        private DataContext _context;

        public MessageService(DataContext context)
        {
            _context = context;
        }

        public async Task<Message> Create(byte[] encryptedContent, Guid senderId, Guid conversationId)
        {
            var message = new Message
            {
                SenderId = senderId,
                ConversationId = conversationId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };

            _context.Add(message);

            await _context.SaveChangesAsync();

            await message.SetEncryptedContent(encryptedContent);

            return message;
        }

        public async Task Delete(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            message.DeleteEncryptedContent();
            _context.Remove(message);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<Message> Retrieve(Guid id)
        {
            return await _context.FindAsync<Message>(id);
        }

        public async Task Edit(Guid id, byte[] newEncryptedContent)
        {
            var message = await _context.FindAsync<Message>(id);

            await message.SetEncryptedContent(newEncryptedContent);
            message.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTheConversationMember(Guid conversationId, Guid userId)
        {
            return await _context.UserConversations.AnyAsync(mc => mc.ConversationId == conversationId && mc.UserId == userId);
        }

        public IEnumerable<Message> GetMessagesFromConversation(Guid conversationId)
        {
            return _context.Messages.Where(m => m.ConversationId == conversationId);
        }
    }
}
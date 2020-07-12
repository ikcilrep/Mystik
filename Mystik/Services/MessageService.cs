using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Models;

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
                SentTime = DateTime.UtcNow
            };

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

        public async Task<IEnumerable<Message>> GetAll()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<Message> Retrieve(Guid id)
        {
            return await _context.FindAsync<Message>(id);
        }

        public Task Update(Guid id, MessagePatch model)
        {
            throw new NotImplementedException();
        }
    }
}
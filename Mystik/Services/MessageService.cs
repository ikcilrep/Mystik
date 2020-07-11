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

        public Task<Message> Create(byte[] encryptedContent, Guid senderId, Guid conversationId)
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

        public async Task<IEnumerable<Message>> GetAll()
        {
            return await _context.Messages.ToListAsync();
        }

        public Task<Message> Retrieve(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, MessagePatch model)
        {
            throw new NotImplementedException();
        }
    }
}
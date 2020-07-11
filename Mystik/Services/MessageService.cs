using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task<IEnumerable<Message>> GetAll()
        {
            throw new NotImplementedException();
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
using System;
using System.Data.Common;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Services;
using Tests.Helpers;

namespace Mystik.Helpers
{
    public class MessageServiceProvider : IDisposable
    {
        private readonly DbConnection _connection;
        protected MessageService MessageService { get; set; }
        protected Conversation Conversation { get; set; }
        protected DataContext Context { get; set; }
        protected int InitialNumberOfMessages { get; set; }

        protected Message Message { get; set; }

        protected MessageServiceProvider()
        {
            MockUserService.ReloadUsers();
            var options = new DbContextOptionsBuilder<DataContext>()
                                   .UseSqlite(CreateInMemoryDatabase())
                                   .Options;
            _connection = RelationalOptionsExtension.Extract(options).Connection;

            Context = new DataContext(options);

            Seed();

            MessageService = new MessageService(Context);
        }

        private void Seed()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            Conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Name = "Conversation",
                PasswordHashData = new byte[] { },
                ModifiedDate = DateTime.UtcNow
            };

            Context.Add(MockUserService.Admin);

            Context.Add(Conversation);

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationManager
            {
                ConversationId = Conversation.Id,
                ManagerId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Message = new Message
            {
                ConversationId = Conversation.Id,
                SenderId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Context.Add(Message);

            Context.SaveChanges();

            InitialNumberOfMessages = Context.Messages.Count();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }



        public void Dispose()
        {
            MessageService.Dispose();
            _connection.Dispose();
        }
    }
}
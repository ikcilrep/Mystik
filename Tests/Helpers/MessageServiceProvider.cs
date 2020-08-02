using System;
using System.Data.Common;
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
        protected DataContext Context { get; set; }

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

            var conversationId = Guid.NewGuid();

            Context.Add(MockUserService.Admin);

            Context.Add(new Conversation
            {
                Id = conversationId,
                Name = "Conversation",
                PasswordHashData = new byte[] { },
                ModifiedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                ConversationId = conversationId,
                UserId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationManager
            {
                ConversationId = conversationId,
                ManagerId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Message = new Message
            {
                ConversationId = conversationId,
                SenderId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Context.Add(Message);

            Context.SaveChanges();
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
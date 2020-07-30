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

namespace Tests
{
    public class ConversationServiceProvider : IDisposable
    {
        private readonly DbConnection _connection;
        protected ConversationService ConversationService { get; }
        protected int InitialNumberOfConversationsMembers { get; set; }
        protected DataContext Context { get; set; }
        protected Conversation Conversation { get; set; }

        protected ConversationServiceProvider()
        {
            MockUserService.ReloadUsers();
            var options = new DbContextOptionsBuilder<DataContext>()
                       .UseSqlite(CreateInMemoryDatabase())
                       .Options;
            _connection = RelationalOptionsExtension.Extract(options).Connection;

            Context = new DataContext(options);

            Seed();

            ConversationService = new ConversationService(Context);
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
            Context.Add(MockUserService.User1);
            Context.Add(MockUserService.User2);

            Context.Add(Conversation);

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = MockUserService.User1.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationManager
            {
                ConversationId = Conversation.Id,
                ManagerId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.SaveChanges();

            InitialNumberOfConversationsMembers = Context.UserConversations.Count();
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose()
        {
            _connection.Dispose();
            Context.Dispose();
        }
    }
}
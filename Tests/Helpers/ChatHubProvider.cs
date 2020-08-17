using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Hubs;
using Mystik.Services;

namespace Tests.Helpers
{
    public class ChatHubProvider : IDisposable
    {
        private readonly DbConnection _connection;
        protected MessageService MessageService { get; set; }
        protected UserService UserService { get; set; }
        protected ConversationService ConversationService { get; set; }
        protected ChatHub ChatHub { get; set; }
        protected DataContext Context { get; set; }

        protected ChatHubProvider()
        {
            MockUserService.ReloadUsers();
            var options = new DbContextOptionsBuilder<DataContext>()
                                   .UseSqlite(CreateInMemoryDatabase())
                                   .Options;
            _connection = RelationalOptionsExtension.Extract(options).Connection;

            Context = new DataContext(options);

            Seed();

            MessageService = new MessageService(Context);
            ConversationService = new ConversationService(Context);
            UserService = new UserService(Context);

            ChatHub = new ChatHub(MessageService, ConversationService, UserService);
        }

        private void Seed()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
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
            UserService.Dispose();
            ConversationService.Dispose();
            _connection.Dispose();
        }
    }
}
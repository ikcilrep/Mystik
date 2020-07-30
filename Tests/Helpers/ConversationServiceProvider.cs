using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Services;
using Tests.Helpers;

namespace Tests
{
    public class ConversationServiceProvider : IDisposable
    {
        private readonly DbConnection _connection;
        protected ConversationService ConversationService { get; }
        protected DataContext Context { get; set; }

        protected ConversationServiceProvider()
        {
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
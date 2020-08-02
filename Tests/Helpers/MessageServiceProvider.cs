using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Services;
using Tests.Helpers;

namespace Mystik.Helpers
{
    public class MessageServiceProvider : IDisposable
    {
        private readonly DbConnection _connection;
        protected MessageService MessageService { get; set; }
        protected DataContext Context { get; set; }

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
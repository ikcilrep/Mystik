using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Services;

namespace Tests.Helpers
{
    public class UserServiceProvider : IDisposable
    {
        private readonly DbConnection _connection;
        public UserService UserService { get; }

        public DataContext Context { get; set; }

        public UserServiceProvider()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                       .UseSqlite(CreateInMemoryDatabase())
                       .Options;
            _connection = RelationalOptionsExtension.Extract(options).Connection;
            Context = new DataContext(options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            UserService = new UserService(Context);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose()
        {
            UserService.Dispose();
            _connection.Dispose();
        }
    }
}
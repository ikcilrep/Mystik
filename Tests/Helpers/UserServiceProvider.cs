using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Services;

namespace Tests.Helpers
{
    public class UserServiceProvider
    {
        public UserService UserService { get; }

        public UserServiceProvider()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                       .UseSqlite(CreateInMemoryDatabase())
                       .Options;
            var connection = RelationalOptionsExtension.Extract(options).Connection;
            var context = new DataContext(options);
            UserService = new UserService(context);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

    }
}
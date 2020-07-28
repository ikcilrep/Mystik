using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Entities;
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
            Seed();

            UserService = new UserService(Context);
        }

        private void Seed()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            Context.Add(MockUserService.Admin);
            Context.Add(MockUserService.User2);

            Context.SaveChanges();
        }

        public void AddFriend()
        {
            Context.Add(new CoupleOfFriends
            {
                Friend1Id = MockUserService.Admin.Id,
                Friend2Id = MockUserService.User2.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new CoupleOfFriends
            {
                Friend1Id = MockUserService.User2.Id,
                Friend2Id = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

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
            UserService.Dispose();
            _connection.Dispose();
        }
    }
}
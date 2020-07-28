using System;
using System.Data.Common;
using System.Linq;
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
        public int InitialNumberOfUsers { get; set; }
        public int InitialNumberOfFriends { get; set; }
        public int InitialNumberOfInvitations { get; set; }
        public Guid ConversationId { get; set; }


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

            InitialNumberOfUsers = Context.Users.Count();
            InitialNumberOfInvitations = Context.Invitations.Count();
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

            InitialNumberOfFriends = Context.Friends.Count();
        }

        public void AddInvitation()
        {
            Context.Add(new Invitation
            {
                InviterId = MockUserService.Admin.Id,
                InvitedId = MockUserService.User2.Id,
                CreatedDate = DateTime.UtcNow
            });
            Context.SaveChanges();

            InitialNumberOfInvitations = Context.Friends.Count();
        }

        public void AddConversation()
        {
            ConversationId = Guid.NewGuid();
            Context.Add(new Conversation
            {
                Id = ConversationId,
                Name = "Conversation",
                PasswordHashData = new byte[] { },
                ModifiedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationManager
            {
                ManagerId = MockUserService.Admin.Id,
                ConversationId = ConversationId,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                UserId = MockUserService.Admin.Id,
                ConversationId = ConversationId,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                UserId = MockUserService.User2.Id,
                ConversationId = ConversationId,
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
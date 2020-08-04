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
        protected UserService UserService { get; }

        protected DataContext Context { get; set; }
        protected int InitialNumberOfUsers { get; set; }
        protected int InitialNumberOfFriends { get; set; }
        protected int InitialNumberOfInvitations { get; set; }
        protected Conversation Conversation { get; set; }


        protected UserServiceProvider()
        {
            MockUserService.ReloadUsers();
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
            Context.Add(MockUserService.User1);
            Context.Add(MockUserService.User2);

            Context.SaveChanges();

            InitialNumberOfUsers = Context.Users.Count();
            InitialNumberOfInvitations = Context.Invitations.Count();
        }

        protected void AddFriend()
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

        protected void AddInvitation()
        {
            Context.Add(new Invitation
            {
                InviterId = MockUserService.Admin.Id,
                InvitedId = MockUserService.User2.Id,
                CreatedDate = DateTime.UtcNow
            });
            Context.SaveChanges();

            InitialNumberOfInvitations = Context.Invitations.Count();
        }

        protected void AddConversation()
        {
            Conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Name = "Conversation",
                PasswordHashData = new byte[] { },
                ModifiedDate = DateTime.UtcNow
            };

            Context.Add(Conversation);

            Context.Add(new ConversationManager
            {
                ManagerId = MockUserService.Admin.Id,
                ConversationId = Conversation.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                UserId = MockUserService.Admin.Id,
                ConversationId = Conversation.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                UserId = MockUserService.User2.Id,
                ConversationId = Conversation.Id,
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
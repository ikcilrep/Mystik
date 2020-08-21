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
        protected CoupleOfFriends Friends1 { get; set; }
        protected CoupleOfFriends Friends2 { get; set; }
        protected Invitation Invitation { get; set; }
        protected UserWithPassword User1 { get; }
        protected UserWithPassword User2 { get; }
        protected UserWithPassword NotExistingUser { get; }
        protected User Admin { get; }

        protected UserServiceProvider()
        {
            User1 = MockUserService.User1;
            User2 = MockUserService.User2;
            Admin = MockUserService.Admin;
            NotExistingUser = MockUserService.NotExistingUser;


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

            Context.Add(Admin);
            Context.Add(User1);
            Context.Add(User2);

            Context.SaveChanges();

            InitialNumberOfUsers = Context.Users.Count();
            InitialNumberOfInvitations = Context.Invitations.Count();
        }

        protected void AddFriend()
        {
            Friends1 = new CoupleOfFriends
            {
                Friend1Id = Admin.Id,
                Friend2Id = User2.Id,
                CreatedDate = DateTime.UtcNow
            };

            Friends2 = new CoupleOfFriends
            {
                Friend1Id = User2.Id,
                Friend2Id = Admin.Id,
                CreatedDate = DateTime.UtcNow
            };

            Context.Add(Friends1);

            Context.Add(Friends2);

            Context.SaveChanges();

            InitialNumberOfFriends = Context.Friends.Count();
        }

        protected void AddInvitation()
        {
            Invitation = new Invitation
            {
                InviterId = Admin.Id,
                InvitedId = User2.Id,
                CreatedDate = DateTime.UtcNow
            };

            Context.Add(Invitation);
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
                ManagerId = Admin.Id,
                ConversationId = Conversation.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                UserId = Admin.Id,
                ConversationId = Conversation.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                UserId = User2.Id,
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
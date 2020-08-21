using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Helpers;
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
        protected Conversation Conversation { get; set; }
        protected Message Message { get; private set; }
        protected Invitation Invitation { get; set; }
        public CoupleOfFriends CoupleOfFriends { get; private set; }
        protected UserWithPassword User1 { get; }
        protected UserWithPassword User2 { get; }
        protected UserWithPassword NotExistingUser { get; }
        protected User Admin { get; }

        protected ChatHubProvider()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";

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

            MessageService = new MessageService(Context);
            ConversationService = new ConversationService(Context);
            UserService = new UserService(Context);

            ChatHub = new ChatHub(MessageService, ConversationService, UserService);
        }

        protected async Task SetMessageContent()
        {
            var encryptedContent = Encoding.UTF8.GetBytes("Awfully old and ugly message.");
            await Message.SetEncryptedContent(encryptedContent);
        }

        protected void AddUser2ToConversation()
        {
            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = User2.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.SaveChanges();
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

            Context.Add(Admin);
            Context.Add(User1);
            Context.Add(User2);

            Context.Add(Conversation);

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = User1.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationManager
            {
                ConversationId = Conversation.Id,
                ManagerId = Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Invitation = new Invitation
            {
                InviterId = User2.Id,
                InvitedId = Admin.Id,
                CreatedDate = DateTime.UtcNow
            };

            CoupleOfFriends = new CoupleOfFriends
            {
                Id = Guid.NewGuid(),
                Friend1Id = User1.Id,
                Friend2Id = Admin.Id,
                CreatedDate = DateTime.UtcNow
            };

            Context.Add(CoupleOfFriends);

            Context.Add(new CoupleOfFriends
            {
                Id = Guid.NewGuid(),
                Friend1Id = Admin.Id,
                Friend2Id = User1.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(Invitation);

            Message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = Conversation.Id,
                SenderId = User1.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            Context.Add(Message);

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
            MessageService.Dispose();
            UserService.Dispose();
            ConversationService.Dispose();
            _connection.Dispose();
        }
    }
}
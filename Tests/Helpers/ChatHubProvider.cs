using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mystik.Data;
using Mystik.Entities;
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
                UserId = MockUserService.User2.Id,
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

            Context.Add(MockUserService.Admin);
            Context.Add(MockUserService.User1);
            Context.Add(MockUserService.User2);

            Context.Add(Conversation);

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationMember
            {
                ConversationId = Conversation.Id,
                UserId = MockUserService.User1.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(new ConversationManager
            {
                ConversationId = Conversation.Id,
                ManagerId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            });

            Invitation = new Invitation
            {
                InviterId = MockUserService.User2.Id,
                InvitedId = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            };

            CoupleOfFriends = new CoupleOfFriends
            {
                Id = Guid.NewGuid(),
                Friend1Id = MockUserService.User1.Id,
                Friend2Id = MockUserService.Admin.Id,
                CreatedDate = DateTime.UtcNow
            };

            Context.Add(CoupleOfFriends);

            Context.Add(new CoupleOfFriends
            {
                Id = Guid.NewGuid(),
                Friend1Id = MockUserService.Admin.Id,
                Friend2Id = MockUserService.User1.Id,
                CreatedDate = DateTime.UtcNow
            });

            Context.Add(Invitation);

            Message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = Conversation.Id,
                SenderId = MockUserService.User1.Id,
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
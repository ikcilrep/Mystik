using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Mystik.Entities;
using Mystik.Helpers;
using Mystik.Hubs;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class ChatHubTest : ChatHubProvider
    {

        [Fact]
        public async Task SendMessage_UserIsInConversation_MessageIsReceived()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.ReceiveMessage(It.IsAny<JsonRepresentableMessage>()));

            var message = Encoding.UTF8.GetBytes("Surprisingly encrypted message.");
            await ChatHub.SendMessage(message, Conversation.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task SendMessage_UserIsNotInConversation_MessageIsNotReceived()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser2Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            var message = Encoding.UTF8.GetBytes("Surprisingly encrypted message.");
            await ChatHub.SendMessage(message, Conversation.Id);

            all.Verify(m => m.ReceiveMessage(It.IsAny<JsonRepresentableMessage>()), Times.Never);
        }

        [Fact]
        public async Task EditMessage_UserIsTheSender_MessageIsEdited()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            mockClients.Setup(m => m.All).Returns(all.Object);

            all.Setup(m => m.EditMessage(It.IsAny<Guid>(), It.IsAny<byte[]>()));

            var oldEncryptedContent = Encoding.UTF8.GetBytes("Awfully old and ugly message.");
            await Message.SetEncryptedContent(oldEncryptedContent);

            var newEncryptedContent = Encoding.UTF8.GetBytes("Brand new message.");
            await ChatHub.EditMessage(Message.Id, newEncryptedContent);

            all.VerifyAll();
        }

        [Fact]
        public async Task EditMessage_UserIsNotTheSender_MessageIsNotEdited()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            var oldEncryptedContent = Encoding.UTF8.GetBytes("Awfully old and ugly message.");
            await Message.SetEncryptedContent(oldEncryptedContent);

            var newEncryptedContent = Encoding.UTF8.GetBytes("Brand new message.");
            await ChatHub.EditMessage(Message.Id, newEncryptedContent);

            all.Verify(m => m.EditMessage(It.IsAny<Guid>(), It.IsAny<byte[]>()), Times.Never);
        }

        [Fact]
        public async Task DeleteMessage_UserIsTheSender_MessageIsDeleted()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.DeleteMessage(It.IsAny<Guid>()));

            var encryptedContent = Encoding.UTF7.GetBytes("Awfully old and ugly message.");
            await Message.SetEncryptedContent(encryptedContent);

            await ChatHub.DeleteMessage(Message.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteMessage_UserIsNotTheSender_MessageIsNotDeleted()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            var encryptedContent = Encoding.UTF7.GetBytes("Awfully old and ugly message.");
            await Message.SetEncryptedContent(encryptedContent);

            await ChatHub.DeleteMessage(Message.Id);

            all.Verify(m => m.DeleteMessage(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CreateConversation_AddedUserJoinsTheConversation()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.JoinConversation(It.IsAny<JsonRepresentableConversation>()));

            await ChatHub.CreateConversation("Some Conversation", new byte[] { }, new List<Guid> { MockUserService.User1.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteConversation_UserIsTheManager_ConversationIsDeleted()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.LeaveConversation(It.IsAny<Guid>()));

            await ChatHub.DeleteConversation(Conversation.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteConversation_UserIsNotTheManager_ConversationIsNotDeleted()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.DeleteConversation(Conversation.Id);

            all.Verify(m => m.LeaveConversation(It.IsAny<Guid>()), Times.Never);

        }

        [Fact]
        public async Task ChangeConversationName_UserIsTheManager_NameIsChanged()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.ChangeConversationName(It.IsAny<Guid>(), It.IsAny<String>()));

            await ChatHub.ChangeConversationName(Conversation.Id, "Brand new conversation name");

            all.VerifyAll();
        }

        [Fact]
        public async Task ChangeConversationName_UserIsNotTheManager_NameIsNotChanged()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.ChangeConversationName(Conversation.Id, "Brand new conversation name");

            all.Verify(m => m.ChangeConversationName(It.IsAny<Guid>(), It.IsAny<String>()), Times.Never);
        }

        [Fact]
        public async Task InviteFriends_InvitedUserReceivesAnInvitation()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.ReceiveInvitation(It.IsAny<Guid>()));

            await ChatHub.InviteFriends(new List<Guid> { MockUserService.User2.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteInvitations_InvitationIsDeleted()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser2Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.DeleteInvitation(It.IsAny<Guid>()));

            await ChatHub.DeleteInvitations(new List<Guid> { Invitation.InvitedId });

            all.VerifyAll();
        }

        [Fact]
        public async Task AddFriend_TheFriendIsAdded()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.User(It.IsAny<string>())).Returns(all.Object);
            all.Setup(m => m.AddFriend(It.IsAny<Guid>()));

            await ChatHub.AddFriend(Invitation.InviterId);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteFriends_TheFriendIsDeleted()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteFriends(new List<Guid> { CoupleOfFriends.Friend2Id });

            all.VerifyAll();
        }
    }
}
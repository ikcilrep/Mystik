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
    }
}
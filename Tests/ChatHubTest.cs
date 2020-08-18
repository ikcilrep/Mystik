using System;
using System.Collections.Generic;
using System.Dynamic;
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
            mockClients.Setup(m => m.All).Returns(all.Object);
            all.Setup(m => m.ReceiveMessage(It.IsAny<JsonRepresentableMessage>()));

            var message = Encoding.UTF8.GetBytes("Surprisingly encrypted message.");
            await ChatHub.SendMessage(message, Conversation.Id);
            
            all.VerifyAll();
        }
    }
}
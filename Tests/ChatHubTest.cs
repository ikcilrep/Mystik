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
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            mockClients.Setup(m => m.All).Returns(all.Object);

            all.Setup(m => m.EditMessage(It.IsAny<Guid>(), It.IsAny<byte[]>()));

            await SetMessageContent();

            var newEncryptedContent = Encoding.UTF8.GetBytes("Brand new message.");
            await ChatHub.EditMessage(Message.Id, newEncryptedContent);

            all.VerifyAll();
        }

        [Fact]
        public async Task EditMessage_UserIsNotTheSender_MessageIsNotEdited()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await SetMessageContent();

            var newEncryptedContent = Encoding.UTF8.GetBytes("Brand new message.");
            await ChatHub.EditMessage(Message.Id, newEncryptedContent);

            all.Verify(m => m.EditMessage(It.IsAny<Guid>(), It.IsAny<byte[]>()), Times.Never);
        }

        [Fact]
        public async Task DeleteMessage_UserIsTheSender_MessageIsDeleted()
        {
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.DeleteMessage(It.IsAny<Guid>()));

            await SetMessageContent();

            await ChatHub.DeleteMessage(Message.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteMessage_UserIsNotTheSender_MessageIsNotDeleted()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await SetMessageContent();

            await ChatHub.DeleteMessage(Message.Id);

            all.Verify(m => m.DeleteMessage(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CreateConversation_AddedUsersJoinTheConversation()
        {
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
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.ChangeConversationName(Conversation.Id, "Brand new conversation name");

            all.Verify(m => m.ChangeConversationName(It.IsAny<Guid>(), It.IsAny<String>()), Times.Never);
        }

        [Fact]
        public async Task InviteFriends_InvitedUsersReceiveAnInvitation()
        {
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
        public async Task DeleteInvitations_InvitationsAreDeleted()
        {
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
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteFriends(new List<Guid> { CoupleOfFriends.Friend2Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task AddConversationMembers_UserIsTheManager_MembersAreAdded()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            await SetMessageContent();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.JoinConversation(It.IsAny<JsonRepresentableConversation>()));
            all.Setup(m => m.AddConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()));

            await ChatHub.AddConversationMembers(Conversation.Id, new List<Guid> { MockUserService.User2.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task AddConversationMembers_UserIsNotTheManager_MembersAreNotAdded()
        {
            ChatHub = ChatHub.WithUser1Identity();

            await SetMessageContent();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.AddConversationMembers(Conversation.Id, new List<Guid> { MockUserService.User2.Id });

            all.Verify(m => m.JoinConversation(It.IsAny<JsonRepresentableConversation>()), Times.Never);
            all.Verify(m => m.AddConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Fact]
        public async Task DeleteConversationMembers_UserIsTheManager_MembersAreDeleted()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(m => m.LeaveConversation(It.IsAny<Guid>()));
            all.Setup(m => m.DeleteConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()));

            await ChatHub.DeleteConversationMembers(Conversation.Id, new List<Guid> { MockUserService.User1.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteConversationMembers_UserIsNotTheManager_MembersAreNotDeleted()
        {
            ChatHub = ChatHub.WithUser1Identity();

            AddUser2ToConversation();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.DeleteConversationMembers(Conversation.Id, new List<Guid> { MockUserService.User2.Id });

            all.Verify(m => m.LeaveConversation(It.IsAny<Guid>()), Times.Never);
            all.Verify(m => m.DeleteConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUser_NicknameIsChanged_FriendsUpdateTheUser()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            {
                mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
                all.Setup(m => m.UpdateFriend(It.IsAny<Guid>(), It.IsAny<string>()));

                await ChatHub.UpdateUser("Brand new nickname", null);

                all.VerifyAll();
            }
        }

        [Fact]
        public async Task UpdateUser_NicknameHasNotChanged_FriendsDoNotUpdateTheUser()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.UpdateUser(null, MockUserService.NotExistingUser.Password);

            all.Verify(m => m.UpdateFriend(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_UserIsDeletedByHimself_FriendsDeleteTheUser()
        {
            ChatHub = ChatHub.WithUser1Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            all.Setup(m => m.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteUser(MockUserService.User1.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteUser_UserIsDeletedByAnOtherUser_FriendsDoNotDeleteTheUser()
        {
            ChatHub = ChatHub.WithUser2Identity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.DeleteUser(MockUserService.User1.Id);

            all.Verify(m => m.DeleteFriend(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_UserIsDeleteByAnAdmin_FriendsDeleteUser()
        {
            ChatHub = ChatHub.WithAdminIdentity();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(m => m.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            all.Setup(m => m.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteUser(MockUserService.User1.Id);
        }
    }
}
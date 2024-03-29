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
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.ReceiveMessage(It.IsAny<JsonRepresentableMessage>()));

            var message = Encoding.UTF8.GetBytes("Surprisingly encrypted message.");
            await ChatHub.SendMessage(message, Conversation.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task SendMessage_UserIsNotInConversation_MessageIsNotReceived()
        {
            ChatHub = ChatHub.WithUserIdentity(User2);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            var message = Encoding.UTF8.GetBytes("Surprisingly encrypted message.");
            await ChatHub.SendMessage(message, Conversation.Id);

            all.Verify(icc => icc.ReceiveMessage(It.IsAny<JsonRepresentableMessage>()), Times.Never);
        }

        [Fact]
        public async Task EditMessage_UserIsTheSender_MessageIsEdited()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            mockClients.Setup(ihcc => ihcc.All).Returns(all.Object);

            all.Setup(icc => icc.EditMessage(It.IsAny<Guid>(), It.IsAny<byte[]>()));

            await SetMessageContent();

            var newEncryptedContent = Encoding.UTF8.GetBytes("Brand new message.");
            await ChatHub.EditMessage(Message.Id, newEncryptedContent);

            all.VerifyAll();
        }

        [Fact]
        public async Task EditMessage_UserIsNotTheSender_MessageIsNotEdited()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await SetMessageContent();

            var newEncryptedContent = Encoding.UTF8.GetBytes("Brand new message.");
            await ChatHub.EditMessage(Message.Id, newEncryptedContent);

            all.Verify(icc => icc.EditMessage(It.IsAny<Guid>(), It.IsAny<byte[]>()), Times.Never);
        }

        [Fact]
        public async Task DeleteMessage_UserIsTheSender_MessageIsDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.DeleteMessage(It.IsAny<Guid>()));

            await SetMessageContent();

            await ChatHub.DeleteMessage(Message.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteMessage_UserIsNotTheSender_MessageIsNotDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await SetMessageContent();

            await ChatHub.DeleteMessage(Message.Id);

            all.Verify(icc => icc.DeleteMessage(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CreateConversation_AddedUsersJoinTheConversation()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.JoinConversation(It.IsAny<JsonRepresentableConversation>()));

            await ChatHub.CreateConversation("Some Conversation", new byte[] { }, new List<Guid> { User1.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteConversation_UserIsTheManager_ConversationIsDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.LeaveConversation(It.IsAny<Guid>()));

            await ChatHub.DeleteConversation(Conversation.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteConversation_UserIsNotTheManager_ConversationIsNotDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.DeleteConversation(Conversation.Id);

            all.Verify(icc => icc.LeaveConversation(It.IsAny<Guid>()), Times.Never);

        }

        [Fact]
        public async Task ChangeConversationName_UserIsTheManager_NameIsChanged()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.ChangeConversationName(It.IsAny<Guid>(), It.IsAny<String>()));

            await ChatHub.ChangeConversationName(Conversation.Id, "Brand new conversation name");

            all.VerifyAll();
        }

        [Fact]
        public async Task ChangeConversationName_UserIsNotTheManager_NameIsNotChanged()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.ChangeConversationName(Conversation.Id, "Brand new conversation name");

            all.Verify(icc => icc.ChangeConversationName(It.IsAny<Guid>(), It.IsAny<String>()), Times.Never);
        }

        [Fact]
        public async Task InviteFriends_InvitedUsersReceiveAnInvitation()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.ReceiveInvitation(It.IsAny<Guid>()));

            await ChatHub.InviteFriends(new List<Guid> { User2.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteInvitations_InvitationsAreDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(User2);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.DeleteInvitation(It.IsAny<Guid>()));

            await ChatHub.DeleteInvitations(new List<Guid> { Invitation.InvitedId });

            all.VerifyAll();
        }

        [Fact]
        public async Task AddFriend_TheFriendIsAdded()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.User(It.IsAny<string>())).Returns(all.Object);
            all.Setup(icc => icc.AddFriend(It.IsAny<Guid>()));

            await ChatHub.AddFriend(Invitation.InviterId);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteFriends_TheFriendIsDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteFriends(new List<Guid> { CoupleOfFriends.Friend2Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task AddConversationMembers_UserIsTheManager_MembersAreAdded()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            await SetMessageContent();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.JoinConversation(It.IsAny<JsonRepresentableConversation>()));
            all.Setup(icc => icc.AddConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()));

            await ChatHub.AddConversationMembers(Conversation.Id, new List<Guid> { User2.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task AddConversationMembers_UserIsNotTheManager_MembersAreNotAdded()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            await SetMessageContent();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.AddConversationMembers(Conversation.Id, new List<Guid> { User2.Id });

            all.Verify(icc => icc.JoinConversation(It.IsAny<JsonRepresentableConversation>()), Times.Never);
            all.Verify(icc => icc.AddConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Fact]
        public async Task DeleteConversationMembers_UserIsTheManager_MembersAreDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.LeaveConversation(It.IsAny<Guid>()));
            all.Setup(icc => icc.DeleteConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()));

            await ChatHub.DeleteConversationMembers(Conversation.Id, new List<Guid> { User1.Id });

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteConversationMembers_UserIsNotTheManager_MembersAreNotDeleted()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            AddUser2ToConversation();

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;

            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.DeleteConversationMembers(Conversation.Id, new List<Guid> { User2.Id });

            all.Verify(icc => icc.LeaveConversation(It.IsAny<Guid>()), Times.Never);
            all.Verify(icc => icc.DeleteConversationMembers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUser_NicknameIsChanged_FriendsUpdateTheUser()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);
            all.Setup(icc => icc.UpdateFriend(It.IsAny<Guid>(), It.IsAny<string>()));

            await ChatHub.UpdateUser("Brand new nickname", null);

            all.VerifyAll();
        }

        [Fact]
        public async Task UpdateUser_NicknameHasNotChanged_FriendsDoNotUpdateTheUser()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.UpdateUser(null, NotExistingUser.Password);

            all.Verify(icc => icc.UpdateFriend(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_UserIsDeletedByHimself_FriendsDeleteTheUser()
        {
            ChatHub = ChatHub.WithUserIdentity(User1);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            all.Setup(icc => icc.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteUser(User1.Id);

            all.VerifyAll();
        }

        [Fact]
        public async Task DeleteUser_UserIsDeletedByAnOtherUser_FriendsDoNotDeleteTheUser()
        {
            ChatHub = ChatHub.WithUserIdentity(User2);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            await ChatHub.DeleteUser(User1.Id);

            all.Verify(icc => icc.DeleteFriend(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_UserIsDeletedByAnAdmin_FriendsDeleteUser()
        {
            ChatHub = ChatHub.WithUserIdentity(Admin);

            var mockClients = new Mock<IHubCallerClients<IChatClient>>();
            var all = new Mock<IChatClient>();
            ChatHub.Clients = mockClients.Object;
            mockClients.Setup(ihcc => ihcc.Users(It.IsAny<IReadOnlyList<string>>())).Returns(all.Object);

            all.Setup(icc => icc.DeleteFriend(It.IsAny<Guid>()));

            await ChatHub.DeleteUser(User1.Id);
        }
    }
}
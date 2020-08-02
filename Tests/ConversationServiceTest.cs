using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Helpers;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class ConversationServiceTest : ConversationServiceProvider
    {
        [Fact]
        public async Task IsTheConversationManager_UserIsManager_ReturnsTrue()
        {
            var userIsManager = await ConversationService.IsTheConversationManager(Conversation.Id, MockUserService.Admin.Id);

            Assert.True(userIsManager);
        }

        [Fact]
        public async Task IsTheConversationManager_UserIsNotManager_ReturnsFalse()
        {
            var userIsManager = await ConversationService.IsTheConversationManager(Conversation.Id, MockUserService.User1.Id);

            Assert.False(userIsManager);
        }

        [Fact]
        public async Task GetNotManagingMembersIds_ReturnsCorrectIds()
        {
            var expectedIds = new List<Guid> { MockUserService.User1.Id };
            var actualIds = await ConversationService.GetNotManagingMembersIds(Conversation.Id);

            Assert.Equal(expectedIds, actualIds);
        }

        [Fact]
        public async Task AddMembers_AddsExactlyOneEntity()
        {
            var usersIds = new List<Guid> { MockUserService.User2.Id };
            await ConversationService.AddMembers(Conversation.Id, usersIds);

            Assert.Equal(InitialNumberOfConversationsMembers + 1, Context.ConversationMembers.Count());
        }

        [Fact]
        public async Task AddMembers_AddsCorrectEntity()
        {
            var usersIds = new List<Guid> { MockUserService.User2.Id };
            await ConversationService.AddMembers(Conversation.Id, usersIds);

            Assert.True(Context.ConversationMembers.Any(cm => cm.Conversation.Id == Conversation.Id
                                                            && cm.UserId == MockUserService.User2.Id));
        }

        [Fact]
        public async Task AddMembers_ReturnsNewMembers()
        {
            var usersIds = new List<Guid> { MockUserService.User2.Id, MockUserService.NotExistingUser.Id };
            var members = await ConversationService.AddMembers(Conversation.Id, usersIds);

            var expectedMembers = new HashSet<Guid> { MockUserService.User2.Id };

            Assert.True(members.ToHashSet().SetEquals(expectedMembers));
        }

        [Fact]
        public async Task DeleteMembers_RemovesExactlyOneEntity()
        {
            var usersIds = new List<Guid> { MockUserService.User1.Id };

            await ConversationService.DeleteMembers(Conversation.Id, usersIds);

            Assert.Equal(InitialNumberOfConversationsMembers - 1, Context.ConversationMembers.Count());
        }

        [Fact]
        public async Task DeleteMembers_RemovesCorrectEntity()
        {
            var usersIds = new List<Guid> { MockUserService.User1.Id };

            await ConversationService.DeleteMembers(Conversation.Id, usersIds);

            Assert.False(Context.ConversationMembers.Any(cm => cm.Conversation.Id == Conversation.Id
                                                            && cm.UserId == MockUserService.User1.Id));
        }

        [Fact]
        public async Task Create_AddsExactlyOneEntity()
        {
            await ConversationService.Create(
                "Conversation1",
                new byte[] { },
                MockUserService.User2.Id);

            Assert.Equal(InitialNumberOfConversations + 1, Context.Conversations.Count());
        }

        [Fact]
        public async Task Create_AddsCorrectEntity()
        {
            var conversation = await ConversationService.Create(
                "Conversation1",
                new byte[] { },
                MockUserService.User2.Id);

            Assert.True(Context.Conversations.Any(c => c.Id == conversation.Id));
        }

        [Fact]
        public async Task Retrieve_ReturnsTheCorrectEntity()
        {
            var conversation = await ConversationService.Retrieve(Conversation.Id);

            Assert.Equal(Conversation, conversation);
        }

        [Fact]
        public async Task ChangeName_ChangesTheCorrectEntity()
        {
            var newName = "NewConversationName";
            await ConversationService.ChangeName(Conversation.Id, newName);

            Assert.True(Context.Conversations.Any(c => c.Id == Conversation.Id && c.Name == newName));
        }

        [Fact]
        public async Task ChangeName_ReturnsConversationMembersIds()
        {
            var newName = "NewConversationName";
            var usersToNotify = await ConversationService.ChangeName(Conversation.Id, newName);

            Assert.Equal(usersToNotify, Conversation.GetMembers());
        }

        [Fact]
        public async Task Delete_RemovesExactlyOneEntity()
        {
            await ConversationService.Delete(Conversation.Id);

            Assert.Equal(InitialNumberOfConversations - 1, Context.Conversations.Count());
        }

        [Fact]
        public async Task Delete_RemovesTheCorrectEntity()
        {
            await ConversationService.Delete(Conversation.Id);

            Assert.False(Context.Conversations.Any(c => c.Id == Conversation.Id));
        }

        [Fact]
        public async Task GetAll_ReturnsTheCorrectEntities()
        {
            var actualEntities = await ConversationService.GetAll();

            var expectedEntities = new List<Conversation> { Conversation };

            Assert.True(expectedEntities.CollectionEqual(actualEntities, c => c.Id));
        }
    }
}
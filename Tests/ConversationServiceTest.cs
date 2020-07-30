using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            Assert.Equal(InitialNumberOfConversationsMembers + 1, Context.UserConversations.Count());
        }

        [Fact]
        public async Task AddMembers_AddsCorrectEntity()
        {
            var usersIds = new List<Guid> { MockUserService.User2.Id };
            await ConversationService.AddMembers(Conversation.Id, usersIds);

            Assert.True(Context.UserConversations.Any(cm => cm.Conversation.Id == Conversation.Id
                                                            && cm.UserId == MockUserService.User2.Id));
        }
    }
}
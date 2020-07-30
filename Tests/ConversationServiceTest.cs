using System;
using System.Collections.Generic;
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
    }
}
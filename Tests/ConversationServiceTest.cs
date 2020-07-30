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
    }
}
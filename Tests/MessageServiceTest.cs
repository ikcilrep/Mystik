using System.Threading.Tasks;
using Mystik.Helpers;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class MessageServiceTest : MessageServiceProvider
    {
        [Fact]
        public async Task IsTheConversationMember_UserIsTheConversationMember_ReturnsTrue()
        {
            var userIsTheConversationMember = await MessageService.IsTheConversationMember(
                Conversation.Id,
                MockUserService.Admin.Id);

            Assert.True(userIsTheConversationMember);
        }
    }
}
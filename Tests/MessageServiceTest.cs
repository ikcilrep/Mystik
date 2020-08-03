using System.Linq;
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

        [Fact]
        public async Task IsTheConversationMember_UserIsNotTheConversationMember_ReturnsFalse()
        {
            var userIsTheConversationMember = await MessageService.IsTheConversationMember(
                Conversation.Id,
                MockUserService.User1.Id);

            Assert.False(userIsTheConversationMember);
        }

        [Fact]
        public async Task Create_AddsExactlyOneEntity()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            await MessageService.Create(new byte[] { }, MockUserService.Admin.Id, Conversation.Id);

            Assert.Equal(InitialNumberOfMessages + 1, Context.Messages.Count());
        }
    }
}
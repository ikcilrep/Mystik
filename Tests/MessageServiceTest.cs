using System.Linq;
using System.Text;
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

        [Fact]
        public async Task Create_AddsCorrectEntity()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";

            var message = await MessageService.Create(
                new byte[] { },
                MockUserService.Admin.Id,
                Conversation.Id);

            Assert.True(Context.Messages.Any(m => m.Id == message.Id));
        }

        [Fact]
        public async Task Retrieve_ReturnsCorrectEntity()
        {
            var message = await MessageService.Retrieve(Message.Id);

            Assert.Equal(Message.Id, message.Id);
        }

        [Fact]
        public async Task Edit_ModifiesTheCorrectEntity()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            var expectedMessageContent = Encoding.UTF8.GetBytes("New message content!");
            await MessageService.Edit(Message.Id, expectedMessageContent);

            var actualMessageContent = await Message.GetEncryptedContent();

            Assert.True(expectedMessageContent.SequenceEqual(actualMessageContent));
        }
    }
}
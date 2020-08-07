using System;
using System.Text;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Helpers;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class MessageTest
    {
        protected Message Message { get; set; }

        public MessageTest()
        {
            Message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = MockUserService.User1.Id,
                ConversationId = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetEncryptedContent_ReturnsCorrectData()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";

            var expectedData = Encoding.UTF8.GetBytes("Miau.");
            await Message.SetEncryptedContent(expectedData);

            var actualData = await Message.GetEncryptedContent();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SetEncryptedContent_ModifiesModifiedDate()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";
            var modifiedDateBefore = Message.ModifiedDate;

            await Message.SetEncryptedContent(new byte[] { });

            Assert.NotEqual(modifiedDateBefore, Message.ModifiedDate);
        }
    }
}
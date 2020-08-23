using System;
using System.IO;
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

        protected UserWithPassword User1 { get; }

        public MessageTest()
        {
            User1 = MockUserService.User1;
            AppSettings.EncryptedMessagesPath = "/tmp";

            Message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = User1.Id,
                ConversationId = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetEncryptedContent_ReturnsCorrectData()
        {
            var expectedData = Encoding.UTF8.GetBytes("Miau.");
            await Message.SetEncryptedContent(expectedData);

            var actualData = await Message.GetEncryptedContent();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SetEncryptedContent_ModifiesModifiedDate()
        {
            var modifiedDateBefore = Message.ModifiedDate;

            await Message.SetEncryptedContent(new byte[] { });

            Assert.NotEqual(modifiedDateBefore, Message.ModifiedDate);
        }

        [Fact]
        public void DeleteEncryptedContent_DeletesData()
        {
            Message.DeleteEncryptedContent();

            Assert.ThrowsAsync<DirectoryNotFoundException>(() => Message.GetEncryptedContent());
        }
    }
}
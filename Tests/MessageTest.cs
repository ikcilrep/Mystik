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
        [Fact]
        public async Task GetEncryptedContent_ReturnsCorrectData()
        {
            AppSettings.EncryptedMessagesPath = "/tmp";

            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = MockUserService.User1.Id,
                ConversationId = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var expectedData = Encoding.UTF8.GetBytes("Miau.");
            await message.SetEncryptedContent(expectedData);

            var actualData = await message.GetEncryptedContent();
            Assert.Equal(expectedData, actualData);
        }
    }
}
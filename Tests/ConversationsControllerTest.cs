using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class ConversationsControllerTest : ConversationsControllerProvider
    {
        [Fact]
        public async Task Get_ReturnsCorrectEntities()
        {
            var actualConversations = await ConversationsController.Get();

            var expectedConversationsIds = new HashSet<Guid> { Conversation.Id };

            var actualConversationsIds = actualConversations.Select(c => c.Id);

            Assert.True(expectedConversationsIds.SetEquals(actualConversationsIds));
        }
    }
}
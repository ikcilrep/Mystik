using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Services;

namespace Mystik.Helpers
{
    public static class IEnumerableExtensions
    {
        public static async Task<List<byte[]>> GetEncryptedContent(this IEnumerable<Message> messages)
        {
            var result = new List<byte[]>();

            foreach (var message in messages.OrderBy(m => m.CreatedDate))
            {
                result.Add(await message.GetEncryptedContent());
            }

            return result;
        }

        public static List<string> ToStringList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(e => e.ToString()).ToList();
        }

        public static async Task<List<Guid>> GetUsersToDelete(
            this IEnumerable<Guid> usersIds,
            Guid conversationId,
            Guid currentUserId,
            IConversationService conversationService)
        {
            var usersToDeleteIds = new List<Guid>();
            var notManagingMembersIds = await conversationService.GetNotManagingMembersIds(conversationId);

            return notManagingMembersIds.Intersect(usersIds).ToList();
        }

        public static async Task<List<object>> GetJsonRepresentableConversations(
            this IEnumerable<UserConversation> userConversations,
            DateTime since)
        {
            var representableConversations = new List<object>();
            foreach (var userConversation in userConversations)
            {
                var representableConversation = await userConversation.Conversation.ToJsonRepresentableObject(since);
                representableConversations.Add(representableConversation);
            }
            return representableConversations;
        }
    }
}
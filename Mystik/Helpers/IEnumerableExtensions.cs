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
        public static async Task<List<JsonRepresentableMessage>> GetJsonRepresentableMessages(this IEnumerable<Message> messages)
        {
            var representableMessages = new List<JsonRepresentableMessage>();

            foreach (var message in messages)
            {
                representableMessages.Add(await message.ToJsonRepresentableObject());
            }

            return representableMessages;
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

        public static async Task<List<JsonRepresentableConversation>> GetJsonRepresentableConversations(
            this IEnumerable<UserConversation> userConversations,
            DateTime since)
        {
            var representableConversations = new List<JsonRepresentableConversation>();
            foreach (var userConversation in userConversations)
            {
                var representableConversation = await userConversation.Conversation.ToJsonRepresentableObject(since);
                representableConversations.Add(representableConversation);
            }
            return representableConversations;
        }

        public static async Task<List<object>> GetJsonRepresentableUsers(this IEnumerable<User> users, DateTime since)
        {
            var representableUsers = new List<object>();
            foreach (var user in users)
            {
                representableUsers.Add(await user.ToJsonRepresentableObject(since));
            }
            return representableUsers;
        }
    }
}
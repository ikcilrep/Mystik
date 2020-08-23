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
            var notManagingMembersIds = await conversationService.GetNotManagingMembersIds(conversationId);

            return notManagingMembersIds.Intersect(usersIds).ToList();
        }

        public static async Task<List<JsonRepresentableConversation>> GetJsonRepresentableConversations(
            this IEnumerable<ConversationMember> userConversations,
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

        public static async Task<List<JsonRepresentableUser>> GetJsonRepresentableUsers(this IEnumerable<User> users, DateTime since)
        {
            var representableUsers = new List<JsonRepresentableUser>();
            foreach (var user in users)
            {
                representableUsers.Add(await user.ToJsonRepresentableObject(since));
            }
            return representableUsers;
        }


        public static async Task<List<JsonRepresentableUser>> GetJsonRepresentableUsers(this IEnumerable<User> users)
        {
            return await users.GetJsonRepresentableUsers(DateTime.UnixEpoch);
        }
        public static bool CollectionEqual<T>(
           this IEnumerable<T> collection1,
           IEnumerable<T> collection2,
           Func<T, Guid> selector)
        {
            var collection1IsEmpty = collection1 == null || !collection1.Any();
            var collection2IsEmpty = collection2 == null || !collection2.Any();

            if (collection1IsEmpty && collection2IsEmpty)
            {
                return true;
            }

            return collection1IsEmpty == collection2IsEmpty
                   && collection1.Select(selector).ToHashSet().SetEquals(collection2.Select(selector));
        }
    }
}
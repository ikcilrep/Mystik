using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Helpers;

namespace Mystik.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<ConversationManager> Managers { get; set; }

        public ICollection<ConversationMember> Members { get; set; }

        public ICollection<Message> Messages { get; set; }

        public byte[] PasswordHashData { get; set; }

        public DateTime ModifiedDate { get; set; }

        public async Task<JsonRepresentableConversation> ToJsonRepresentableObject(DateTime since)
        {
            return new JsonRepresentableConversation
            {
                Id = Id,
                Name = Name,
                PasswordHashData = PasswordHashData,
                Messages = await Messages.Where(m => m.ModifiedDate > since).GetJsonRepresentableMessages(),
                Members = Members.Where(cm => cm.CreatedDate > since).Select(cm => cm.User.GetPublicData()),
                Managers = Managers.Where(cm => cm.CreatedDate > since).Select(cm => cm.Manager.GetPublicData()),
            };
        }

        public async Task<JsonRepresentableConversation> ToJsonRepresentableObject()
        {
            return await ToJsonRepresentableObject(DateTime.UnixEpoch);
        }

        public bool HasBeenModifiedSince(DateTime since)
        {
            return ModifiedDate > since
                   || Members.Any(cm => cm.CreatedDate > since || cm.User.ModifiedDate > since)
                   || Managers.Any(cm => cm.CreatedDate > since)
                   || Messages.Any(m => m.ModifiedDate > since);
        }

        public bool IsMember(Guid userId) => Members.Any(cm => cm.UserId == userId);

        public IReadOnlyList<string> GetMembers()
        {
            return Members.Select(m => m.UserId).ToStringList();
        }

        public override bool Equals(object obj)
        {
            return obj is Conversation conversation &&
                   Id.Equals(conversation.Id) &&
                   Name == conversation.Name &&
                   Managers.CollectionEqual(conversation.Managers, cm => cm.ManagerId) &&
                   Members.CollectionEqual(conversation.Members, cm => cm.UserId) &&
                   Messages.CollectionEqual(conversation.Messages, m => m.Id) &&
                   PasswordHashData.SequenceEqual(conversation.PasswordHashData) &&
                   ModifiedDate == conversation.ModifiedDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Managers, Members, Messages, PasswordHashData, ModifiedDate);
        }
    }
}
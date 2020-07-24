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

        public ICollection<ManagedConversation> ManagedConversations { get; set; }

        public ICollection<UserConversation> UserConversations { get; set; }

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
                Members = UserConversations.Where(uc => uc.CreatedDate > since).Select(uc => uc.User.GetPublicData()),
                Managers = ManagedConversations.Where(mc => mc.CreatedDate > since).Select(uc => uc.Manager.GetPublicData()),
            };
        }

        public async Task<JsonRepresentableConversation> ToJsonRepresentableObject()
        {
            return await ToJsonRepresentableObject(DateTime.UnixEpoch);
        }

        public bool HasBeenModifiedSince(DateTime since)
        {
            return ModifiedDate > since
                   || UserConversations.Any(uc => uc.CreatedDate > since || uc.User.ModifiedDate > since)
                   || ManagedConversations.Any(mc => mc.CreatedDate > since)
                   || Messages.Any(m => m.ModifiedDate > since);
        }

        public bool IsMember(Guid userId) => UserConversations.Any(uc => uc.UserId == userId);

        [NotMapped]
        public IReadOnlyList<string> Members => UserConversations.ToStringList();
    }
}
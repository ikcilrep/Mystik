using System;
using System.Collections.Generic;
using System.Linq;

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

        public object ToJsonRepresentableObject()
        {
            return new
            {
                Id = Id,
                Name = Name,
                PasswordHashData = PasswordHashData,
                Messages = Messages.OrderBy(m => m.SentTime).Select(m => m.GetEncryptedContent()),
                Users = UserConversations.Select(uc => uc.UserId),
                Managers = ManagedConversations.Select(uc => uc.AdminId),
            };
        }
    }
}
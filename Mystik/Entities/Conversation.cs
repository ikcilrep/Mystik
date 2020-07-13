using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<object> ToJsonRepresentableObject()
        {
            var messages = new List<byte[]>();

            foreach (var message in Messages.OrderBy(m => m.SentTime))
            {
                messages.Add(await message.GetEncryptedContent());
            }

            return new
            {
                Id = Id,
                Name = Name,
                PasswordHashData = PasswordHashData,
                Messages = messages,
                Users = UserConversations.Select(uc => uc.UserId),
                Managers = ManagedConversations.Select(uc => uc.ManagerId),
            };
        }
    }
}
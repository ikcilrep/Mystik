using System.ComponentModel.DataAnnotations;
using Mystik.Entities;

namespace Mystik.Models.Conversation
{
    public class Patch
    {
        [Required]
        public string Name { get; set; }

        public Entities.Conversation ToConversation(Entities.Conversation originalConversation)
        {
            return new Entities.Conversation
            {
                Name = Name == null ? originalConversation.Name : Name,
                Id = originalConversation.Id,
                ManagedConversations = originalConversation.ManagedConversations,
                UserConversations = originalConversation.UserConversations,
                Messages = originalConversation.Messages,
                PasswordHashData = originalConversation.PasswordHashData,
            };
        }
    }
}
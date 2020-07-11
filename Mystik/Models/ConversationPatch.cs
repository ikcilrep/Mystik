using System.ComponentModel.DataAnnotations;
using Mystik.Entities;

namespace Mystik.Models
{
    public class ConversationPatch
    {
        [Required]
        public string Name { get; set; }

        public Conversation ToConversation(Conversation originalConversation)
        {
            return new Conversation
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
using System;

namespace Mystik.Entities
{
    public class UserConversation
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid ConversationId { get; set; }
    }
}
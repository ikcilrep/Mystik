
using System;

namespace Mystik.Entities
{
    public class ManagedConversation
    {
        public User Admin { get; set; }
        public Guid AdminId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid ConversationId { get; set; }
    }
}
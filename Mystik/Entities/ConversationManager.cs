using System;

namespace Mystik.Entities
{
    public class ConversationManager
    {
        public User Manager { get; set; }
        public Guid ManagerId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid ConversationId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
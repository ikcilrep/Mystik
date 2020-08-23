using System;

namespace Mystik.Entities
{
    public class ConversationMember
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid ConversationId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
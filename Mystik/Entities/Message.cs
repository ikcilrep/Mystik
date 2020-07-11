using System;

namespace Mystik.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public string EncryptedContentPath { get; set; }
        public User Sender { get; set; }
        public Conversation Conversation { get; set; }
        public DateTime SentTime { get; set; }
    }
}
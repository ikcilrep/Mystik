using System;
using System.IO;
using System.Threading.Tasks;
using Mystik.Helpers;

namespace Mystik.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public DateTime SentTime { get; set; }

        public async Task SetEncryptedContent(byte[] encryptedContent)
        {
            var path = Path.Combine(AppSettings.EncryptedMessagesPath, Id.ToString());
            await File.WriteAllBytesAsync(path, encryptedContent);
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public String EncryptedContentPath => Path.Combine(AppSettings.EncryptedMessagesPath, Id.ToString());

        public async Task SetEncryptedContent(byte[] encryptedContent)
        {
            await File.WriteAllBytesAsync(EncryptedContentPath, encryptedContent);
        }

        public async Task<byte[]> GetEncryptedContent() => await File.ReadAllBytesAsync(EncryptedContentPath);

        public void DeleteEncryptedContent() => File.Delete(EncryptedContentPath);
    }
}
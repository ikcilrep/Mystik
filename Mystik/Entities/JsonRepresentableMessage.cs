using System;

namespace Mystik.Entities
{
    public class JsonRepresentableMessage
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] EncryptedContent { get; set; }
    }
}
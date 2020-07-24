using System;
using System.Collections.Generic;

namespace Mystik.Entities
{
    public class JsonRepresentableConversation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] PasswordHashData { get; set; }
        public IEnumerable<JsonRepresentableMessage> Messages { get; set; }
        public IEnumerable<UserPublicData> Members { get; set; }
        public IEnumerable<UserPublicData> Managers { get; set; }
    }
}
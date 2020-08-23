using System;
using System.Collections.Generic;

namespace Mystik.Entities
{
    public class JsonRepresentableUser
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public IEnumerable<UserPublicData> Friends { get; set; }
        public IEnumerable<UserPublicData> Inviters { get; set; }
        public IEnumerable<UserPublicData> Invited { get; set; }
        public IEnumerable<JsonRepresentableConversation> Conversations { get; set; }
    }
}
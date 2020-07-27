using System;
using System.Collections.Generic;

namespace Mystik.Entities
{
    public class UserRelatedEntities
    {
        public IEnumerable<Guid> FriendsIds { get; set; }
        public IEnumerable<Guid> InvitedIds { get; set; }
        public IEnumerable<Guid> InvitersIds { get; set; }
        public IEnumerable<Guid> ConversationIds { get; set; }
        public IEnumerable<Guid> ConversationMembersIds { get; set; }
        public IEnumerable<Guid> ConversationManagersIds { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystik.Models
{
    public class UserRelatedEntities
    {
        public IEnumerable<Guid> FriendsIds { get; set; }
        public IEnumerable<Guid> InvitedIds { get; set; }
        public IEnumerable<Guid> InvitersIds { get; set; }
        public IEnumerable<Guid> ConversationIds { get; set; }
        public IEnumerable<Guid> ConversationMembersIds { get; set; }
        public IEnumerable<Guid> ConversationManagersIds { get; set; }

        public override bool Equals(object obj)
        {
            return obj is UserRelatedEntities other
                   && FriendsIds.SequenceEqual(other.FriendsIds)
                   && InvitedIds.SequenceEqual(other.InvitedIds)
                   && InvitersIds.SequenceEqual(other.InvitersIds)
                   && ConversationIds.SequenceEqual(other.ConversationIds)
                   && ConversationMembersIds.SequenceEqual(other.ConversationMembersIds)
                   && ConversationManagersIds.SequenceEqual(other.ConversationManagersIds);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FriendsIds, InvitedIds, InvitersIds, ConversationIds, ConversationMembersIds, ConversationManagersIds);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
using System;

namespace Mystik.Entities
{
    public class InvitedUser
    {
        public Guid InviterId { get; set; }
        public User Inviter { get; set; }
        public Guid InvitedId { get; set; }
        public User Invited { get; set; }
    }
}
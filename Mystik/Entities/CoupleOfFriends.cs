using System;

namespace Mystik.Entities
{
    public class CoupleOfFriends
    {
        public Guid Friend1Id { get; set; }
        public User Friend1 { get; set; }
        public Guid Friend2Id { get; set; }
        public User Friend2 { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
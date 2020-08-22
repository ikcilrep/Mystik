using System;
using System.Collections.Generic;
using Mystik.Entities;

namespace Tests.Helpers
{
    public class MockUserService
    {
        public HashSet<User> Users { get; set; }
        public HashSet<CoupleOfFriends> Friends { get; set; }

        public static User Admin => new User("Adamek", "Adam", "Kaczka1%3")
        {
            Id = Guid.NewGuid(),
            Role = Role.Admin
        };
        public static UserWithPassword User1 => new UserWithPassword(
            "Kacperek",
            "Kacper",
            "#Myszka456",
            Guid.NewGuid());
        public static UserWithPassword User2 => new UserWithPassword(
            "Oliwierek",
            "Oliwier",
            "Gruszka!789",
            Guid.NewGuid());
        public static UserWithPassword NotExistingUser => new UserWithPassword(
            "Lukaszek",
            "Lukasz",
            "Jablko&101112",
            Guid.NewGuid());
   }
}
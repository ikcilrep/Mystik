using System;
using System.Collections.Generic;
using Mystik.Entities;

namespace Tests.Helpers
{
    public class MockUserService
    {
        public HashSet<User> Users { get; set; }
        public HashSet<CoupleOfFriends> Friends { get; set; }

        private static User _admin = new User("Adamek", "Adam", "Kaczka1%3")
        {
            Id = Guid.NewGuid(),
            Role = Role.Admin
        };

        private static UserWithPassword _user1 = new UserWithPassword(
            "Kacperek",
            "Kacper",
            "#Myszka456",
            Guid.NewGuid());


        private static UserWithPassword _user2 = new UserWithPassword(
            "Oliwierek",
            "Oliwier",
            "Gruszka!789",
            Guid.NewGuid());

        private static UserWithPassword _notExistingUser = new UserWithPassword(
            "Lukaszek",
            "Lukasz",
            "Jablko&101112",
            Guid.NewGuid());


        public static User Admin => _admin;
        public static UserWithPassword User1 => _user1;
        public static UserWithPassword User2 => _user2;
        public static UserWithPassword NotExistingUser => _notExistingUser;

        public static void ReloadUsers()
        {
            _admin = new User("Adamek", "Adam", "Kaczka1%3")
            {
                Id = Guid.NewGuid(),
                Role = Role.Admin
            };

            _user1 = new UserWithPassword(
               "Kacperek",
               "Kacper",
               "#Myszka456",
               Guid.NewGuid());


            _user2 = new UserWithPassword(
               "Oliwierek",
               "Oliwier",
               "Gruszka!789",
               Guid.NewGuid());

            _notExistingUser = new UserWithPassword(
               "Lukaszek",
               "Lukasz",
               "Jablko&101112",
               Guid.NewGuid());
        }
    }
}
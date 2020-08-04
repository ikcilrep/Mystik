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
            Id = Guid.Parse("6c554aa4-3fd8-48d4-a0d8-13164f172d0c"),
            Role = Role.Admin
        };

        private static UserWithPassword _user1 = new UserWithPassword(
            "Kacperek",
            "Kacper",
            "#Myszka456",
            "4192105b-3256-40e2-9efb-eef265e5eaa6");


        private static UserWithPassword _user2 = new UserWithPassword(
            "Oliwierek",
            "Oliwier",
            "Gruszka!789",
            "60398e2a-4439-46bf-9101-e26ea63d5326");

        private static UserWithPassword _notExistingUser = new UserWithPassword(
            "Lukaszek",
            "Lukasz",
            "Jablko&101112",
            "68fa61ae-8c9b-4470-8fd7-a36e1e14c035");


        public static User Admin => _admin;
        public static UserWithPassword User1 => _user1;
        public static UserWithPassword User2 => _user2;
        public static UserWithPassword NotExistingUser => _notExistingUser;

        public static void ReloadUsers()
        {
            _admin = new User("Adamek", "Adam", "Kaczka1%3")
            {
                Id = Guid.Parse("6c554aa4-3fd8-48d4-a0d8-13164f172d0c"),
                Role = Role.Admin
            };

            _user1 = new UserWithPassword(
               "Kacperek",
               "Kacper",
               "#Myszka456",
               "4192105b-3256-40e2-9efb-eef265e5eaa6");


            _user2 = new UserWithPassword(
               "Oliwierek",
               "Oliwier",
               "Gruszka!789",
               "60398e2a-4439-46bf-9101-e26ea63d5326");

            _notExistingUser = new UserWithPassword(
               "Lukaszek",
               "Lukasz",
               "Jablko&101112",
               "68fa61ae-8c9b-4470-8fd7-a36e1e14c035");
        }
    }
}
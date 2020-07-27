using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Helpers;
using Mystik.Models.User;
using Mystik.Services;

namespace Tests.Helpers
{
    public class MockUserService : IUserService
    {
        public HashSet<User> Users { get; set; }

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

        public MockUserService()
        {
            Users = new HashSet<User>() { Admin, User1, User2 };
        }

        public Task<User> Authenticate(string username, string password)
        {
            return Task.Run(() => Users.FirstOrDefault(user => user.Username == username
            && Hashing.DoesPasswordMatch(password, user.PasswordSalt, user.PasswordHash)));
        }

        public Task<User> Create(string nickname, string username, string password)
        {
            var user = new User(nickname, username, password);
            Users.Add(user);
            return Task.Run(() => user);
        }

        public Task<IReadOnlyList<string>> Delete(Guid id)
        {
            var user = Users.First(u => u.Id == id);
            Users.Remove(user);
            return Task.Run(() => user.GetFriends());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            IEnumerable<User> users = Users;
            return Task.Run(() => users);
        }

        public Task<User> Retrieve(Guid id)
        {
            return Task.Run(() => Users.FirstOrDefault(user => user.Id == id));
        }

        public Task<IReadOnlyList<string>> Update(Guid id, string newNickname, string newPassword)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            var usersToNotify = newNickname == user.Nickname ? new List<string>() : user.GetFriends();

            user.Nickname = newNickname;
            user.SetPassword(newPassword);

            Users.RemoveWhere(user => user.Id == id);
            Users.Add(user);
            return Task.Run(() => usersToNotify);
        }

        public Task AddFriend(Guid inviterId, Guid invitedId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFriends(Guid id, List<Guid> usersIds)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<string>> InviteFriends(Guid inviterId, List<Guid> invitedIds)
        {
            throw new NotImplementedException();
        }

        public Task DeleteInvitations(Guid inviterId, List<Guid> invitedIds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInvited(Guid inviterId, Guid invitedId)
        {
            throw new NotImplementedException();
        }

        public Task<UserRelatedEntities> GetNotExisting(Guid id, UserRelatedEntities model)
        {
            throw new NotImplementedException();
        }
    }
}
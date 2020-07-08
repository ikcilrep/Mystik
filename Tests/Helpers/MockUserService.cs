using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models;
using Mystik.Services;

namespace Tests.Helpers
{
    public class MockUserService : IUserService
    {
        public HashSet<User> Users { get; set; }

        public MockUserService()
        {
            Users = new HashSet<User> {
                new User("Adamek", "Adam", "Kaczka123") {
                    Id = Guid.Parse("6c554aa4-3fd8-48d4-a0d8-13164f172d0c"),
                    Role = "Admin"
                    },
                new User("Kacperek", "Kacper", "Myszka456") {
                    Id = Guid.Parse("4192105b-3256-40e2-9efb-eef265e5eaa6")
                    },
                new User("Oliwierek", "Oliwier", "Gruszka789") {
                    Id = Guid.Parse("60398e2a-4439-46bf-9101-e26ea63d5326")
                    },
            };
        }

        public Task<User> Authenticate(string username, string password)
        {
            return Task.Run(() => Users.FirstOrDefault(user => user.Username == username));
        }

        public Task<User> Create(string nickname, string username, string password)
        {
            var user = new User(nickname, username, password);
            Users.Add(user);
            return Task.Run(() => user);
        }

        public Task Delete(Guid id)
        {
            Users.RemoveWhere(user => user.Id == id);
            return Task.CompletedTask;
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

        public Task Update(Guid id, User user)
        {
            Users.RemoveWhere(user => user.Id == id);
            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task Update(Guid id, UserPatch model)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            var updatedUser = model.ToUser(user);

            Users.RemoveWhere(user => user.Id == id);
            Users.Add(updatedUser);
            return Task.CompletedTask;
        }
    }
}
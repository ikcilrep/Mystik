using System;
using Mystik.Entities;

namespace Tests.Helpers
{

    public class UserWithPassword : User
    {
        public string Password { get; set; }

        public UserWithPassword(
            string nickname,
            string username,
            string password,
            Guid id) : base(nickname, username, password)
        {
            Password = password;
            Id = id;
        }
    }
}
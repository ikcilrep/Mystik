using System;
using Mystik.Helpers;

namespace Mystik.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public User(string nickname, string username, string password)
        {
            Nickname = nickname;
            Username = username;
            Hashing.CreatePasswordHash(password, out byte[] passwordSalt, out byte[] passwordHash);
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
            Role = Entities.Role.User;
        }

        public User() { }


    }
}

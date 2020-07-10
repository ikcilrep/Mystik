using System;
using System.ComponentModel.DataAnnotations;
using Mystik.Entities;

namespace Mystik.Models
{
    public class UserPatch
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public virtual User ToUser(User originalUser)
        {
            var user = new User
            {
                Id = originalUser.Id,
                Username = originalUser.Username,
                Nickname = Nickname == null ? originalUser.Nickname : Nickname,
                Role = originalUser.Role
            };
            if (Password == null)
            {
                user.PasswordHash = originalUser.PasswordHash;
                user.PasswordSalt = originalUser.PasswordSalt;
            }
            else
            {
                user.SetPassword(Password);
            }
            return user;
        }
    }
}
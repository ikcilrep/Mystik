using System;

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
    }
}

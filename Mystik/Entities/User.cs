﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public ICollection<UserConversation> UserConversations { get; set; }
        public ICollection<InvitedUser> InvitedUsers { get; set; }
        public ICollection<InvitedUser> Invitations { get; set; }
        public ICollection<ManagedConversation> ManagedConversations { get; set; }
        public ICollection<Message> Messages { get; set; }

        public User(string nickname, string username, string password)
        {
            Nickname = nickname;
            Username = username;
            SetPassword(password);
            Role = Entities.Role.User;
        }

        public User() { }

        public void SetPassword(string password)
        {
            Hashing.CreatePasswordHash(password, out byte[] passwordSalt, out byte[] passwordHash);
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
        }

        public override bool Equals(object obj)
        {
            return obj is User user
                   && user.Id == Id
                   && user.Nickname == Nickname
                   && user.Username == Username
                   && user.Role == Role
                   && user.PasswordHash.SequenceEqual(PasswordHash)
                   && user.PasswordSalt.SequenceEqual(PasswordSalt);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nickname, Username, Role, PasswordHash, PasswordSalt);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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
        public ICollection<Invitation> SentInvitations { get; set; }
        public ICollection<Invitation> ReceivedInvitations { get; set; }
        public ICollection<CoupleOfFriends> Friends1 { get; set; }
        public ICollection<CoupleOfFriends> Friends2 { get; set; }
        public ICollection<ManagedConversation> ManagedConversations { get; set; }
        public ICollection<Message> Messages { get; set; }

        public IReadOnlyList<string> GetFriends()
        {
            return Friends1.Select(cof => cof.Friend2Id).ToStringList();
        }

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

        public async Task<object> ToJsonRepresentableObject(DateTime since)
        {
            return new
            {
                Id = Id,
                Nickname = Nickname,
                Username = Username,
                Friends = Friends1.Where(cof => cof.CreatedDate >= since).Select(cof => cof.Friend2Id),
                ReceivedInvitations = ReceivedInvitations.Where(i => i.CreatedDate > since)
                                                         .Select(i => i.InviterId),
                SentInvitations = SentInvitations.Where(i => i.CreatedDate > since)
                                                 .Select(i => i.InvitedId),
                Conversations = await UserConversations.Where(uc => uc.CreatedDate > since
                                                                    || uc.Conversation.HasBeenModifiedSince(since))
                                                       .GetJsonRepresentableConversations(since)
            };
        }

        public object GetPublicData()
        {
            return new
            {
                Id = Id,
                Nickname = Nickname,
            };
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

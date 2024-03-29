﻿using System;
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
        public ICollection<ConversationMember> ParticipatedConversations { get; set; }
        public ICollection<Invitation> SentInvitations { get; set; }
        public ICollection<Invitation> ReceivedInvitations { get; set; }
        public ICollection<CoupleOfFriends> Friends1 { get; set; }
        public ICollection<CoupleOfFriends> Friends2 { get; set; }
        public ICollection<ConversationManager> ManagedConversations { get; set; }
        public ICollection<Message> Messages { get; set; }
        public DateTime ModifiedDate { get; set; }

        public IReadOnlyList<string> GetRelatedUsers()
        {
            var friendsIds = Friends1.Select(cof => cof.Friend1Id).ToStringList();
            var conversationMembersIds = ParticipatedConversations.SelectMany(cm => cm.Conversation.GetMembers())
                                                                  .Where(id => id != Id.ToString());

            var relatedUsersIdsSet = conversationMembersIds.ToHashSet();
            relatedUsersIdsSet.UnionWith(friendsIds);

            return relatedUsersIdsSet.ToStringList();
        }

        public User(string nickname, string username, string password)
        {
            Nickname = nickname;
            Username = username;
            SetPassword(password);
            Role = Entities.Role.User;
            ModifiedDate = DateTime.UtcNow;
        }

        public User() { }

        public void SetPassword(string password)
        {
            Hashing.CreatePasswordHash(password, out byte[] passwordSalt, out byte[] passwordHash);
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
        }

        public async Task<JsonRepresentableUser> ToJsonRepresentableObject(DateTime since)
        {
            return new JsonRepresentableUser
            {
                Id = Id,
                Nickname = Nickname,
                Username = Username,
                Friends = Friends1.Where(cof => cof.CreatedDate > since || cof.Friend1.ModifiedDate > since).Select(cof => cof.Friend1.GetPublicData()),
                Inviters = ReceivedInvitations.Where(i => i.CreatedDate > since || i.Inviter.ModifiedDate > since)
                                                         .Select(i => i.Inviter.GetPublicData()),
                Invited = SentInvitations.Where(i => i.CreatedDate > since || i.Invited.ModifiedDate > since)
                                                 .Select(i => i.Invited.GetPublicData()),
                Conversations = await ParticipatedConversations.Where(cm => cm.CreatedDate > since
                                                                    || cm.Conversation.HasBeenModifiedSince(since))
                                                       .GetJsonRepresentableConversations(since)
            };
        }

        public UserPublicData GetPublicData()
        {
            return new UserPublicData
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

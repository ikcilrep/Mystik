﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Helpers;

namespace Mystik.Services
{
    public class UserService : IUserService
    {

        private static readonly Regex _lowerCase = new Regex(@"[a-z]");
        private static readonly Regex _upperCase = new Regex(@"[A-Z]");
        private static readonly Regex _digit = new Regex(@"\d");
        private static readonly Regex _specialCharacter = new Regex(@"[#$^+=!*()@%&]");

        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                return null;
            }

            if (!Hashing.DoesPasswordMatch(password, user.PasswordSalt, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User> Create(string nickname, string username, string password)
        {
            await ValidateCredentials(nickname, username, password);

            var user = new User(nickname, username, password);
            _context.Add(user);

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Retrieve(Guid id)
        {
            return await _context.Users.Include(u => u.Friends1)
                                       .Include(u => u.ManagedConversations)
                                       .Include(u => u.UserConversations)
                                            .ThenInclude(u => u.Conversation)
                                            .ThenInclude(u => u.UserConversations)
                                       .Include(u => u.UserConversations)
                                           .ThenInclude(u => u.Conversation)
                                           .ThenInclude(u => u.Messages)
                                       .Include(u => u.UserConversations)
                                           .ThenInclude(u => u.Conversation)
                                           .ThenInclude(u => u.ManagedConversations)
                                       .Include(u => u.ReceivedInvitations)
                                       .Include(u => u.SentInvitations)
                                       .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IReadOnlyList<string>> Delete(Guid id)
        {
            var user = await _context.Users.Include(u => u.Friends1)
                                           .Include(u => u.ManagedConversations)
                                           .ThenInclude(mc => mc.Conversation)
                                           .ThenInclude(c => c.ManagedConversations)
                                           .Include(u => u.UserConversations)
                                           .ThenInclude(uc => uc.Conversation)
                                           .ThenInclude(c => c.UserConversations)
                                           .FirstAsync(u => u.Id == id);
            var usersToNotify = user.GetFriends();

            var abandonedManagedConversations = user.ManagedConversations.Where(mc => mc.Conversation.ManagedConversations.Count == 1)
                                                                         .Select(mc => mc.Conversation);

            var abandonedConversations = user.UserConversations.Where(mc => mc.Conversation.UserConversations.Count == 1)
                                                               .Select(mc => mc.Conversation);

            _context.RemoveRange(abandonedManagedConversations);
            _context.RemoveRange(abandonedConversations);

            _context.Remove(user);
            await _context.SaveChangesAsync();

            return usersToNotify;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<string>> Update(Guid id, string newNickname, string newPassword)
        {
            var user = await _context.Users.Include(u => u.Friends1).FirstOrDefaultAsync(u => u.Id == id);
            var usersToNotify = newNickname == user.Nickname ? new List<string>() : user.GetFriends();

            if (newNickname != null)
            {
                ValidateNickname(newNickname);
                user.Nickname = newNickname;
            }

            if (newPassword != null)
            {
                ValidatePassword(newPassword);
                user.SetPassword(newPassword);
            }

            await _context.SaveChangesAsync();

            return usersToNotify;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private void ValidateNickname(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                throw new AppException("Nickname is required.");
            }

            if (nickname[0] == '@')
            {
                throw new AppException("Nickname mustn't begin with \"@\".");
            }

            if (nickname.Length > 64)
            {
                throw new AppException("Nickname mustn't be longer than sixty four characters.");
            }
        }
        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required.");
            }
            if (password.Length < 8)
            {
                throw new AppException("Password must be at least eight characters long.");
            }

            if (!_digit.IsMatch(password))
            {
                throw new AppException("Password must contain at least one digit.");
            }

            if (!_lowerCase.IsMatch(password))
            {
                throw new AppException("Password must contain at least one lower case letter.");
            }

            if (!_upperCase.IsMatch(password))
            {
                throw new AppException("Password must contain at least one upper case letter.");
            }

            if (!_specialCharacter.IsMatch(password))
            {
                throw new AppException("Password must contain at least one special character.");
            }
        }

        private async Task ValidateCredentials(string nickname, string username, string password)
        {
            ValidateNickname(nickname);
            ValidatePassword(password);

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new AppException("Username is required.");
            }

            if (username[0] == '@')
            {
                throw new AppException("Username mustn't begin with \"@\".");
            }

            if (username.Length > 64)
            {
                throw new AppException("Username mustn't be longer than sixty four characters.");
            }

            if (await _context.Users.AnyAsync(user => user.Username == username))
            {
                throw new AppException($"Username \"{username}\" has already been taken.");
            }
        }

        public async Task AddFriend(Guid inviterId, Guid invitedId)
        {
            _context.Add(new CoupleOfFriends
            {
                Friend1Id = inviterId,
                Friend2Id = invitedId,
                CreatedDate = DateTime.UtcNow
            });

            _context.AddRange(new CoupleOfFriends
            {
                Friend1Id = invitedId,
                Friend2Id = inviterId,
                CreatedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriends(Guid id, List<Guid> usersIds)
        {
            var existingFriends = _context.Friends.Where(f => (f.Friend1Id == id && usersIds.Contains(f.Friend2Id))
                                                              || (f.Friend2Id == id && usersIds.Contains(f.Friend1Id)));
            _context.RemoveRange(existingFriends);

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<string>> InviteFriends(Guid inviterId, List<Guid> invitedIds)
        {
            var existingNotInvitedUsers = _context.Users.Include(u => u.Friends1)
                                                        .Include(u => u.ReceivedInvitations)
                                                        .Include(u => u.SentInvitations)
                                                        .Where(u => invitedIds.Contains(u.Id)
                                                                    && u.Friends1.All(cof => cof.Friend2Id != inviterId)
                                                                    && u.ReceivedInvitations.All(iu => iu.InviterId != inviterId)
                                                                    && u.SentInvitations.All(iu => iu.InvitedId != inviterId))
                                                        .Select(u => u.Id);
            _context.AddRange(existingNotInvitedUsers.Select(invitedId => new Invitation
            {
                InviterId = inviterId,
                InvitedId = invitedId,
                CreatedDate = DateTime.UtcNow
            }));

            await _context.SaveChangesAsync();

            return existingNotInvitedUsers.ToStringList();
        }

        public async Task DeleteInvitations(Guid inviterId, List<Guid> invitedIds)
        {
            var existingInvitations = _context.Invitations.Where(i => inviterId == i.InviterId
                                                                     && invitedIds.Contains(i.InvitedId));
            _context.RemoveRange(existingInvitations);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserInvited(Guid inviterId, Guid invitedId)
        {
            return await _context.Invitations.AnyAsync(i => i.InviterId == inviterId && i.InvitedId == invitedId);
        }
    }
}

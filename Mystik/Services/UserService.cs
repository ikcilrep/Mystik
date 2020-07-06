using System;
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

            if (await _context.SaveChangesAsync() != 1)
            {
                throw new Exception("Failed to write to database.");
            }

            return user;
        }

        public async Task<User> Retrieve(Guid id)
        {
            return await _context.FindAsync<User>(new object[1] { id });
        }

        public async Task Update(Guid id, User updatedUser)
        {
            var user = await _context.FindAsync<User>(new object[1] { id });
            _context.Entry(user).CurrentValues.SetValues(updatedUser);

            if (await _context.SaveChangesAsync() != 1)
            {
                throw new Exception("Failed to write to database.");
            }

        }

        public async Task Delete(Guid id)
        {
            var user = new User { Id = id };
            _context.Remove(user);

            if (await _context.SaveChangesAsync() != 1)
            {
                throw new Exception("Failed to write to database.");
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task Update(Guid id, string nickname, string password)
        {
            var user = await _context.FindAsync<User>(new object[1] { id });
            if (nickname != null)
            {
                user.Nickname = nickname;
            }

            if (password != null)
            {
                user.SetPassword(password);
            }

            if ((nickname != null || password != null) && await _context.SaveChangesAsync() != 1)
            {
                throw new Exception("Failed to write to database.");
            }
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

    }
}

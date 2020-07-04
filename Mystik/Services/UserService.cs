using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Mystik.Data;
using Mystik.Entities;
using Mystik.Helpers;

namespace Mystik.Services
{
    public class UserService : IUserService
    {
        private const int Iterations = 200000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
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

            if (!DoesPasswordMatch(password, user.PasswordSalt, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User> Create(string username, string password)
        {
            ValidateCredentials(username, password);

            byte[] passwordSalt = new byte[SaltSize];
            CreatePasswordHash(password, passwordSalt, out byte[] passwordHash);
            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
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

        private void ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new AppException("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required.");
            }

            if (username[0] == '@')
            {
                throw new AppException("Username mustn't begin with \"@\".");
            }

            if (username.Length > 64)
            {
                throw new AppException("Username mustn't be longer than sixty four characters.");
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

            if (_context.Users.Any(user => user.Username == username))
            {
                throw new AppException($"Username \"{username}\" has already been taken.");
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

        private byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: Iterations,
                    numBytesRequested: HashSize);
        }

        private bool DoesPasswordMatch(string password, byte[] salt, byte[] hash)
        {
            return HashPassword(password, salt).SequenceEqual(hash);
        }


        private void CreatePasswordHash(string password, byte[] salt, out byte[] hash)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            hash = HashPassword(password, salt);
        }
    }
}

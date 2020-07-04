using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Mystik.Entities;
using Mystik.Helpers;

namespace Mystik.Data
{
    public class UserRepository : IUserRepository
    {
        private const int Iterations = 200000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        private DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Create(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required.");
            }

            if (_context.Users.Any(user => user.Username == username))
            {
                throw new AppException($"Username \"{username}\" has already been taken.");
            }

            byte[] passwordHash;
            byte[] passwordSalt = new byte[SaltSize];
            HashPassword(password, passwordSalt, out passwordHash);
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

        private static void HashPassword(string password, byte[] salt, out byte[] hash)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: HashSize);
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
    }
}

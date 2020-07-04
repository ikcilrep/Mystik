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
            var user = new User()
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _context.AddAsync(user);

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

        Task<User> IUserRepository.Retrieve(Guid id)
        {
            throw new NotImplementedException();
        }

        Task IUserRepository.Update(Guid id, User user)
        {
            throw new NotImplementedException();
        }

        Task IUserRepository.Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

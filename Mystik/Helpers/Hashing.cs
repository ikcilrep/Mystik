using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Mystik.Helpers
{
    public static class Hashing
    {
        private const int SaltSize = 16;
        private const int Iterations = 200000;
        private const int HashSize = 32;

        private static byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: Iterations,
                    numBytesRequested: HashSize);
        }

        public static bool DoesPasswordMatch(string password, byte[] salt, byte[] hash)
        {
            return HashPassword(password, salt).SequenceEqual(hash);
        }


        public static void CreatePasswordHash(string password, out byte[] salt, out byte[] hash)
        {
            salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            hash = HashPassword(password, salt);
        }

    }
}

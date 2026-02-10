using System;
using System.Security.Cryptography;
using System.Text;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.BusinessLogicLayer.Services
{
    // Simple SHA256 hasher for example purposes only.
    public class SimplePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string hashedPassword, string providedPassword)
        {
            var providedHash = Hash(providedPassword);
            return hashedPassword == providedHash;
        }
    }
}
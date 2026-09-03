using System;
using System.Security.Cryptography;
using System.Text;

namespace GameHub.Data
{
    /// <summary>Password hashing helper (SHA-256, lower-case hex).</summary>
    public static class Security
    {
        public static string Hash(string plainText)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText ?? string.Empty));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}

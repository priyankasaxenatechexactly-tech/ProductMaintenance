using System.Security.Cryptography;
using System.Text;

namespace ProductMaintenance.Utils
{
    public static class Sha256Hasher
    {
        public static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("X2")); // uppercase hex
            return sb.ToString();
        }
    }
}

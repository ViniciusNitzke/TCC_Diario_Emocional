using System.Security.Cryptography;
using System.Text;
using DiarioEmocional.Api.Application.Security;

namespace DiarioEmocional.Api.Infrastructure.Security;

public sealed class Sha256PasswordHasher : IPasswordHasher
{
    public (string Hash, string Salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        return (CreateHash(password, salt), Convert.ToBase64String(salt));
    }

    public bool Verify(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var attempt = CreateHash(password, saltBytes);
        return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(hash), Convert.FromBase64String(attempt));
    }

    private static string CreateHash(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var buffer = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, buffer, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, buffer, salt.Length, passwordBytes.Length);
        return Convert.ToBase64String(SHA256.HashData(buffer));
    }
}

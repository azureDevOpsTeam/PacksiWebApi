#region Usings

using System.Security.Cryptography;
using System.Text;

#endregion

namespace ApplicationLayer.Extensions;

public static class HashGenerator
{
    private const string SecurityPepper = "C4qWkpZ8CEs3A9Ks";

    public static string GenerateSHA256HashWithSalt(string password, out string securityStamp)
    {
        var saltBytes = new byte[16];
        RandomNumberGenerator.Fill(saltBytes);

        securityStamp = Convert.ToBase64String(saltBytes);
        var combinedBytes = CombinePasswordAndSalt(password, securityStamp);

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(combinedBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public static string GenerateHashChangePassword(string password, string securityStamp)
    {
        var combinedBytes = CombinePasswordAndSalt(password, securityStamp);

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(combinedBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword, string securityStamp)
    {
        var combinedBytes = CombinePasswordAndSalt(password, securityStamp);

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(combinedBytes);
        var computedHash = Convert.ToBase64String(hashBytes);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(hashedPassword)
        );
    }

    private static byte[] CombinePasswordAndSalt(string password, string securityStamp)
    {
        var salt = Encoding.UTF8.GetBytes(securityStamp + SecurityPepper);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var combined = new byte[passwordBytes.Length + salt.Length];

        Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);

        return combined;
    }
}
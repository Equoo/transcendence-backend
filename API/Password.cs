using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Identity;
using BC = BCrypt.Net.BCrypt;

namespace KeepGrouped.API.Password;


public class Hash256
{
    public static string GetHashSha256(string text)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += string.Format("{0:x2}", x);
        }
        return hashString;
    }
}

public class KeepGroupedPasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        string password_hashed = BC.HashPassword(password);
        return password_hashed;
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {

        if (BC.Verify(providedPassword, hashedPassword))
            return PasswordVerificationResult.Success;
        else
            return PasswordVerificationResult.Failed;
    }
}


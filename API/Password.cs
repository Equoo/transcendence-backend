using System.Data.SqlTypes;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Identity;
using BC = BCrypt.Net.BCrypt;

namespace KeepGrouped.API.Password;


public class KeepGroupedPasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        string password_hashed = BC.HashPassword(password);
        return password_hashed;
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        if (hashedPassword == HashPassword(user, providedPassword))
            return PasswordVerificationResult.Success;
        else   
            return PasswordVerificationResult.Failed;
    }
}
    

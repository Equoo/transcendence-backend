using System.Security.Cryptography;
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

/// <summary>Handler-to-endpoint carrier: the endpoint alone turns the tokens into cookies.</summary>
public sealed record AuthenticatedUser(LoginResponse User, IssuedTokens Tokens);

public sealed class LoginUserCommand(KeepGroupedDb db, IPasswordHasher<User> pass) : IHandler
{
    public async Task<Result<AuthenticatedUser>> ExecuteAsync(LoginRequest req)
    {
        User? user_db = await db.Users.SingleOrDefaultAsync(u => u.UserName == req.UserName);

        if (user_db is null)
        {
            return AuthProblems.InvalidCredentials();
        }

        if (pass.VerifyHashedPassword(user_db, user_db.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
        {
            return AuthProblems.InvalidCredentials();
        }

        string acess_token = Token.CreateAccess(user_db.Id);

        string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
        string refresh_token = Token.CreateRefresh(id);

        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Hash256.GetHashSha256(id),
            UserId = user_db.Id,
            ExpireAt = DateTime.Now.AddDays(7).Kind
        });

        await db.SaveChangesAsync();

        return new AuthenticatedUser(LoginResponse.FromEntity(user_db), new IssuedTokens(acess_token, refresh_token));
    }
}

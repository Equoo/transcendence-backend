using System.Security.Cryptography;
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

/// <summary>Handler-to-endpoint carrier: the endpoint alone turns the tokens into cookies.</summary>
public sealed record RefreshedSession(RefreshResponse User, IssuedTokens Tokens);

public sealed class RefreshTokensCommand(KeepGroupedDb db, TokenProvider provider) : IHandler
{
    public async Task<Result<RefreshedSession>> ExecuteAsync(string? cookieRefresh)
    {
        if (cookieRefresh is null)
        {
            return AuthProblems.RefreshTokenMissing();
        }

        if (!provider.IsValid(cookieRefresh))
        {
            return AuthProblems.RefreshTokenInvalid();
        }

        string refresh_id = Hash256.GetHashSha256(provider.ReadFirstClaim(cookieRefresh));

        // Can have many if your are log in different computer in the same account
        RefreshToken? refresh_db = await db.RefreshTokens.FirstOrDefaultAsync(o => o.Id == refresh_id);
        if (refresh_db is null)
        {
            return AuthProblems.RefreshTokenUnknown();
        }

        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == refresh_db.UserId);
        if (user is null)
        {
            return AuthProblems.RefreshTokenUnknown();
        }

        string new_acess = provider.CreateAccess(refresh_db.UserId);

        string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
        string new_refresh = provider.CreateRefresh(id);

        db.RefreshTokens.Remove(refresh_db);
        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Hash256.GetHashSha256(id),
            UserId = refresh_db.UserId,
            ExpireAt = DateTime.Now.AddDays(7).Kind
        });

        await db.SaveChangesAsync();

        return new RefreshedSession(RefreshResponse.FromEntity(user), new IssuedTokens(new_acess, new_refresh));
    }
}

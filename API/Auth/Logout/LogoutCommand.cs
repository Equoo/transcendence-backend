using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class LogoutCommand(KeepGroupedDb db, TokenProvider provider) : IHandler
{
    public async Task<Result> ExecAsync(string id, string? refresh)
    {
        if (refresh is null)
        {
            return Result.OK;
        }

        var refreshId = Hash256.GetHashSha256(provider.ReadFirstClaim(refresh));
        RefreshToken? token = await db.RefreshTokens.SingleOrDefaultAsync(r => r.Id == refreshId);

        if (token is null)
        {
            return Result.OK;
        }

        db.RefreshTokens.Remove(token);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}

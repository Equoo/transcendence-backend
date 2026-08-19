using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class LogoutQuery(KeepGroupedDb db): IHandler
{
    public async Task<Result> ExecAsync(string id)
    {
        RefreshToken? refresh = await db.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == id);

        if (refresh is null)
        {
            return TokensProblems.NotFound(id);
        }

        db.RefreshTokens.Remove(refresh);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}

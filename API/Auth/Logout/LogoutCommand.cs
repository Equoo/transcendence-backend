using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class LogoutCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecAsync(string id)
    {


        RefreshToken? refresh = await db.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == id);

        if (refresh is null)
        {
            return Result.OK;
        }

        db.RefreshTokens.Remove(refresh);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}

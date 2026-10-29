using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class LogoutAllCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecAsync(string id)
    {

        var tokens = await db.RefreshTokens.Where(r => r.UserId == id).ToListAsync();

        db.RefreshTokens.RemoveRange(tokens);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}

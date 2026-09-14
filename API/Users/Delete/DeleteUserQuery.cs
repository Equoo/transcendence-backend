
using KeepGrouped.API;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteUserQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string id)
    {
        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return UserProblems.NotFound(id);
        }

        RefreshToken? refresh = await db.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == id);

        if (refresh is not null)
        {
            db.RefreshTokens.Remove(refresh);
        }

        db.Users.Remove(user);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
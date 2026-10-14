using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

public sealed class GetMeCommand(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecuteAsync()
    {
        User? db_user = await db.Users.SingleOrDefaultAsync(u => u.Id == tk.User.Id);

        if (db_user is null)
        {
            return UserProblems.NotFound(tk.User.Id);
        }

        db_user.ExpireAt = DateTime.Now.AddYears(1).ToUniversalTime();

        await db.SaveChangesAsync();
        return Result.OK;
    }
}
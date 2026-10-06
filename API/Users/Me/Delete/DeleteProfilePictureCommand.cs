using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteAvatarCommand(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecuteAsync()
    {
        User? db_user = await db.Users.SingleOrDefaultAsync(u => u.Id == tk.User.Id);

        if (db_user is not null)
        {
            if (db_user.Avatar is not null)
            {
                db_user.Avatar = null;
                await db.SaveChangesAsync();
            }
            return Result.OK;
        }
        return UserProblems.NotFound(tk.User.Id);
    }
}
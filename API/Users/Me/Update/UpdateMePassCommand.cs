
using System.Net.Security;
using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;

public sealed class UpdateMePassCommand(KeepGroupedDb db, KeepGroupedPasswordHasher pass, TokenContext tk) : IHandler
{
    public async Task<Result> ExecuteAsync(UpdateMePassRequest req)
    {
        User? db_user = await db.Users.SingleOrDefaultAsync(usr => usr.Id == tk.User.Id);

        if (db_user is null)
        {
            return UserProblems.NotFound(tk.User.Id);
        }

        if (pass.VerifyHashedPassword(tk.User, tk.User.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
        {
            return MeProblems.InvalidPassword();
        }

        db_user.PasswordHash = pass.HashPassword(tk.User, req.NewPassword);

        await db.SaveChangesAsync();
        return Result.OK;
    }
}
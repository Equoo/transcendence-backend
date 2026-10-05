
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class ResetPasswordQuery(KeepGroupedDb db, KeepGroupedPasswordHasher hash, IOptions<AuthenticationOptions> option) : IHandler
{
    public async Task<Result> ExecAsync(string id, string newPassword)
    {
        User? db_user = await db.Users.SingleOrDefaultAsync(u => u.Id == id);

        if (db_user is null)
        {
            return UserProblems.NotFound(id);
        }

        if (option.Value.DefaultAdminLogin == db_user.UserName)
        {
            return UserProblems.PasswordCannotBeChanged();
        }

        db_user.PasswordHash = hash.HashPassword(db_user, newPassword);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
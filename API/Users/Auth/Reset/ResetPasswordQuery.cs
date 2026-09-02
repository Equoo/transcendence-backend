
using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class ResetPasswordQuery(KeepGroupedDb db, IPasswordHasher<User> hash): IHandler
{
    public async Task<Result> ExecAsync(string id, string newPassword)
    {
        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == id);
    
        if (user is null)
        {
            return UserProblems.NotFound(id);
        }

        user.PasswordHash = hash.HashPassword(user, newPassword);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
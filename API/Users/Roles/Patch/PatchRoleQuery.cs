
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Roles;

public sealed class PatchRoleQuery(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecuteAsync(string IdUser, string IdRole)
    {
        User? user = await db.Users
        .Include(u => u.Role)
        .SingleOrDefaultAsync(u => u.Id == IdUser);

        if (tk.User.Id == IdUser)
        {
            return UserProblems.CannotChangeSelf();
        }

        if (user is null)
        {
            return UserProblems.NotFound(IdUser);
        }

        Role? role = await db.Roles
        .SingleOrDefaultAsync(r => r.Id == IdRole);

        if (role is null)
        {
            return RoleProblems.NotFound(IdRole);
        }

        if (tk.User.Role.Ranking >= role.Ranking)
        {
            return UserProblems.NotAuthorized();
        }

        user.Role = role;

        await db.SaveChangesAsync();
        return Result.OK;
    }
}


using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class CreateRoleQuery(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecAsync(string name)
    {

        Role? role_db = await db.Roles.SingleOrDefaultAsync(r => r.Name == name);

        if (role_db is not null)
        {
            return RoleProblems.NameAlreadyUsed(name);
        }

        Role role = new(name, tk.User.Role.Ranking + 1);

        db.Roles.Add(role);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
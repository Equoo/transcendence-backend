
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class CreateRoleQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecAsync(string name)
    {

        int count_role = await db.Roles.CountAsync(r => r.Name != "\\(*-*)/");

        Role? role_db = await db.Roles.SingleOrDefaultAsync(r => r.Name == name);

        if (role_db is not null)
        {
            return RoleProblems.NameAlreadyUsed(name);
        }

        Role role = new(name, count_role + 1);

        db.Roles.Add(role);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
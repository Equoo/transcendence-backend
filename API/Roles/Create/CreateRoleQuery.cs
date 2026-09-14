
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class CreateRoleQuery(KeepGroupedDb db): IHandler
{
    public async Task<Result> ExecAsync(string name)
    {

        Role? role_db = await db.Roles.SingleOrDefaultAsync(r => r.Name == name);

        if (role_db is not null)
        {
            return RoleProblems.NameAlreadyUsed(name);
        }

        Role role = new(name);

        db.Roles.Add(role);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
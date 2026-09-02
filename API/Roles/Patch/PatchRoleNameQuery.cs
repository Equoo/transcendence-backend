
using System.Data;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class PatchRoleNameQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecAsync(string id, string name)
    {
        Role? role = await db.Roles.SingleOrDefaultAsync(r => r.Id == id);

        Role? check_name = await db.Roles.SingleOrDefaultAsync(r => r.Name == name);

        if (check_name is not null)
        {
            return RoleProblems.NameAlreadyUsed(name);
        }

        if (role is null)
        {
            return RoleProblems.NotFound(id);
        }

        role.Name = name;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
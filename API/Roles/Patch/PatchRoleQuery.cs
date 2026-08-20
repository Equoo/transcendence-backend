
using System.Data;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class PatchRoleQuery(KeepGroupedDb db): IHandler
{
    public async Task<Result> ExecAsync(string id, int perm)
    {
        Role? role = await db.Roles.SingleOrDefaultAsync(r => r.Id == id);

        if (role is null)
        {
            return RoleProblems.NotFound(id);
        }            

        role.Permission = perm;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}

using System.Data;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class PatchRoleNameQuery(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecAsync(string id, string name)
    {
        Role? db_role = await db.Roles.SingleOrDefaultAsync(r => r.Id == id);
        Role? check_name = await db.Roles.SingleOrDefaultAsync(r => r.Name == name);

        if (check_name is not null)
        {
            return RoleProblems.NameAlreadyUsed(name);
        }

        if (db_role is null)
        {
            return RoleProblems.NotFound(id);
        }

        if (tk.User.Role.Ranking >= db_role.Ranking)
        {
            return RoleProblems.RankingTooLow();
        }

        db_role.Name = name;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
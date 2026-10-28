
using System.Data;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class PatchRolePermQuery(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecAsync(string id, Perms perm)
    {
        Role? db_role = await db.Roles.SingleOrDefaultAsync(r => r.Id == id);

        if (db_role is null)
        {
            return RoleProblems.NotFound(id);
        }

        Console.WriteLine("\n\n\n\n\n" + (int)perm + "\n\n\n\n\n");
        Console.WriteLine("\n\n\n\n\n" + (int)tk.User.Role.Permission + "\n\n\n\n\n");
        Console.WriteLine("\n\n\n\n\n" + (int)(tk.User.Role.Permission & perm) + "\n\n\n\n\n");



        if (tk.User.Role.Ranking >= db_role.Ranking || (tk.User.Role.Permission & perm) == 0)
        {
            return RoleProblems.RankingTooLow();
        }

        db_role.Permission = perm;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}

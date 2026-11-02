using System.Net;
using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Roles;
using Microsoft.EntityFrameworkCore;

public sealed class RankingControlCommand(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecAsync(string id, string direction)
    {
        List<String> diff = ["up", "down"];



        if (!diff.Contains(direction))
        {
            return RoleProblems.OpNotFound(direction);
        }

        var op = direction == "up" ? -1 : +1;

        Role? db_role = await db.Roles.SingleOrDefaultAsync(r => r.Id == id);

        if (db_role is null)
        {
            return RoleProblems.NotFound();
        }

        if (tk.User.Role.Ranking >= db_role.Ranking) return RoleProblems.RankingTooLow();

        Role? db_seconde = await db.Roles.SingleOrDefaultAsync(r => r.Ranking == db_role.Ranking + op);

        if (db_seconde is null)
        {
            return RoleProblems.NotFound();
        }

        if (db_role.Ranking + op <= tk.User.Role.Ranking) return RoleProblems.RankingTooLow();

        db_seconde.Ranking -= op;
        db_role.Ranking += op;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
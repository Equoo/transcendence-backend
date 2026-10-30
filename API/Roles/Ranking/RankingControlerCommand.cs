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

        Role db_role = await db.Roles.SingleAsync(r => r.Id == id);

        if (tk.User.Role.Ranking >= db_role.Ranking)
        {
            return RoleProblems.RankingTooLow();
        }

        Role db_seconde = await db.Roles.SingleAsync(r => r.Ranking == db_role.Ranking + op);

        db_seconde.Ranking -= op;
        db_role.Ranking += op;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
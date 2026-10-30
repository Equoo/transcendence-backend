using System.Data;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class DeleteRoleQuery(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecAsync(string id)
    {
        Role? db_role = await db.Roles.Include(r => r.Users).SingleOrDefaultAsync(r => r.Id == id);

        if (db_role is null)
        {
            return RoleProblems.NotFound(id);
        }


        if (tk.User.Role.Ranking >= db_role.Ranking)
        {
            return RoleProblems.RankingTooLow();
        }

        if (db_role.Name == Role.SuperRole)
        {
            return RoleProblems.NotAllowed();
        }

        db.Roles.Remove(db_role);

        List<Role> list_change = await db.Roles.Where(r => r.Ranking > db_role.Ranking).ToListAsync();

        foreach (Role rl in list_change)
        {
            rl.Ranking--;
        }

        if (db_role.Users.Count > 0)
        {
            Role? db_member = await db.Roles.SingleOrDefaultAsync(r => r.Name == Role.SuperRole);

            if (db_member is null)
            {
                return RoleProblems.NotFound();
            }

            foreach (User usr in db_role.Users)
            {
                usr.Role = db_member;
            }
        }

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
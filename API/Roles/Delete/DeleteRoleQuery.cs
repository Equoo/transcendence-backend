using System.Data;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class DeleteRoleQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecAsync(string id)
    {
        Role? db_role = await db.Roles.Include(r => r.Users).SingleOrDefaultAsync(r => r.Id == id);

        if (db_role is null)
        {
            return RoleProblems.NotFound(id);
        }

        db.Roles.Remove(db_role);

        if (db_role.Users.Count > 0)
        {
            Role? db_member = await db.Roles.SingleOrDefaultAsync(r => r.Name == "Member");

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
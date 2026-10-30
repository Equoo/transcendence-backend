
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class ListRoleQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<RoleResponse>>> ExecuteAsync()
    {
        List<Role> roles = await db.Roles
                    .Where(r => r.Name != Role.SuperRole)
                    .OrderBy(r => r.Ranking)
                    .ToListAsync();

        return roles.Select(RoleResponse.FromEntity).ToList();
    }
}

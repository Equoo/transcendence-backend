
using System.Collections.Immutable;
using KeepGrouped.API.Roles;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class ListRoleQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<RoleResponse>>> ExecuteAsync()
    {
        List<Role> roles = await db.Roles
                    .OrderBy(r => r.Name)
                    .ToListAsync();


        return roles.Select(RoleResponse.FromEntity).ToList();
    }
}
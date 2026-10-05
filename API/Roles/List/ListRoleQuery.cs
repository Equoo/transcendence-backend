
using System.Collections.Immutable;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Roles;

public sealed class ListRoleQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<RoleResponse>>> ExecuteAsync()
    {
        List<Role> roles = await db.Roles
                    .Where(r => r.Name != "\\(*-*)/")
                    .OrderBy(r => r.Name)
                    .ToListAsync();


        return roles.Select(RoleResponse.FromEntity).ToList();
    }
}
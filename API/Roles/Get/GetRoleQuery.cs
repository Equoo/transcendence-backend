
using KeepGrouped.API.Roles;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public sealed class GetRoleQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<Role>>> ExecuteAsync()
    {
        List<Role> roles = await db.Roles
                    .OrderBy(r => r.Name)
                    .ToListAsync();
        return roles;
    }
}
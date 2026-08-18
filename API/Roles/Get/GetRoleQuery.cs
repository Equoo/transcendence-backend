
using KeepGrouped.API.Roles;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class GetRoleQuery(KeepGroupedDb db): IHandler
{
    public async Task<Result<List<Role>>> ExecuteAsync()
    {
        List<Role> roles = await db.Roles.ToListAsync();
        
        return roles;
    }
}
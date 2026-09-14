using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class ListUsersQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListUsersResponse>>> ExecuteAsync()
    {
        var users = await db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .OrderBy(u => u.UserName)
            .ToListAsync();


        return users.Select(ListUsersResponse.FromEntity).ToList();
    }
}

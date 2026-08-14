using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class ListUsersQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListUsersResponse>>> ExecuteAsync()
    {
        var users = await db.Users.ToListAsync();

        return users.Select(ListUsersResponse.FromEntity).ToList();
    }
}

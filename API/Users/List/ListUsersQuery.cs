using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class ListUsersQuery(KeepGroupedDb db, IOptions<AuthenticationOptions> options) : IHandler
{

    public async Task<Result<List<ListUsersResponse>>> ExecuteAsync()
    {



        var users = await db.Users
            .AsNoTracking()
            .Where(u => u.UserName != options.Value.DefaultAdminLogin)
            .Include(u => u.Role)
            .OrderBy(u => u.UserName)
            .ToListAsync();


        return users.Select(ListUsersResponse.FromEntity).ToList();
    }
}

using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class GetUserQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<GetUserResponse>> ExecuteAsync(string id)
    {
        var user = await db.Users
            .SingleOrDefaultAsync(u => u.Id == id);

        return user is null ? UserProblems.NotFound(id) : GetUserResponse.FromEntity(user);
    }
}

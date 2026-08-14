using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class GetUserQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<GetUserResponse>> ExecuteAsync(string id)
    {
        var user = await db.Users
            .Where(u => u.Id == id)
            .Select(u => new GetUserResponse(
                u.Id,
                u.UserName))
            .FirstOrDefaultAsync();

        return user is null ? UserProblems.NotFound(id) : user;
    }
}

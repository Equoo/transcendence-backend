using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class ListMyTokensQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListMyTokensResponse>>> ExecuteAsync(string userId)
    {
        List<RefreshToken> refreshes = await db.RefreshTokens.Where(o => o.UserId == userId).ToListAsync();

        return refreshes.Select(ListMyTokensResponse.FromEntity).ToList();
    }
}

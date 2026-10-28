using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Storage;

public sealed class ListFilesQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListFilesResponse>>> ExecuteAsync()
    {
        var files = await db.Files
            .AsNoTracking()
            .Include(f => f.Creator).ThenInclude(u => u.Role)
            .Where(f => f.Name.StartsWith("/"))
            .ToListAsync();

        return files.Select(ListFilesResponse.FromEntity).ToList();
    }
}

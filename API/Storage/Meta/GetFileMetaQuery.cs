using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Storage;

public sealed class GetFileMetaQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<GetFileMetaResponse>> ExecuteAsync(string key)
    {
        var filedb = await db.Files
            .AsNoTracking()
            .Include(f => f.Creator).ThenInclude(u => u.Role)
            .SingleOrDefaultAsync(f => f.Key == key);

        return filedb is null ? StorageProblems.FileNotFound(key) : GetFileMetaResponse.FromEntity(filedb);
    }
}

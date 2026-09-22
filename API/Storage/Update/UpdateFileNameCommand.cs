using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Storage;

public sealed class UpdateFileNameCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result<GetFileMetaResponse>> ExecuteAsync(string key, string name)
    {
        var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
        if (filedb is null)
        {
            return StorageProblems.FileNotFound(key);
        }

        filedb.Name = name;
        filedb.LastUpdated = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return GetFileMetaResponse.FromEntity(filedb);
    }
}

using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Storage;

public sealed class DeleteFileCommand(IStorage storage, KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string key)
    {
        var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
        if (filedb is null)
        {
            return StorageProblems.FileNotFound(key);
        }

        var res = await storage.DeleteAsync(key);
        if ((int)res.Code >= 400)
        {
            return StorageProblems.DeleteFailed((int)res.Code);
        }

        db.Files.Remove(filedb);
        await db.SaveChangesAsync();
        return Result.Success;
    }
}

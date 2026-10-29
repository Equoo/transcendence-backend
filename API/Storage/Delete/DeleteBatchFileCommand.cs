using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.AiBackend.Delete;
using KeepGrouped.API.Rag;

namespace KeepGrouped.API.Storage;

public sealed class DeleteBatchFileCommand(IStorage storage, KeepGroupedDb db, RagQueue rag) : IHandler
{
    public async Task<Result> ExecuteAsync(IEnumerable<string> keys)
    {
        var filesdb = await db.Files.Where(fi => keys.Contains(fi.Key)).ToListAsync();
        var res = await storage.DeleteBatchAsync(keys);

        filesdb.ForEach(rag.Delete);
        db.Files.RemoveRange(filesdb);
        await db.SaveChangesAsync();
        return Result.OK;
    }
}

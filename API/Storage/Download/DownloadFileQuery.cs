using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Storage;

public sealed record FileDownload(Stream Content, string ContentType, string FileName, DateTime LastModified, string ETag);

public sealed class DownloadFileQuery(IStorage storage, KeepGroupedDb db) : IHandler
{
    public async Task<Result<FileDownload>> ExecuteAsync(string key)
    {
        var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
        if (filedb is null)
        {
            return StorageProblems.FileNotFound(key);
        }

        var res = await storage.DownloadAsync(key);
        if ((int)res.Code >= 400)
        {
            return StorageProblems.DownloadFailed((int)res.Code);
        }

        return new FileDownload(res.Stream, filedb.ContentType, filedb.Name, filedb.LastUpdated, filedb.ETag);
    }
}

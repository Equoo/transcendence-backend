using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Storage;

public sealed class UploadFileCommand(IStorage storage, KeepGroupedDb db) : IHandler
{
    public async Task<Result<UploadFileResponse>> ExecuteAsync(Stream content, string name, string contentType, long length, User creator)
    {
        if (length <= 0)
        {
            return StorageProblems.EmptyFile();
        }

        var res = await storage.UploadAsync(content, contentType);
        if ((int)res.Code >= 400)
        {
            return StorageProblems.UploadFailed((int)res.Code);
        }

        var filedb = new StorageFile()
        {
            Key = res.Key,
            Name = name,
            ETag = res.ETag,
            ContentType = contentType,
            Length = length,
            LastUpdated = DateTime.UtcNow,
            Creator = creator
        };

        db.Files.Add(filedb);

        await db.SaveChangesAsync();
        return UploadFileResponse.FromEntity(filedb);
    }
}

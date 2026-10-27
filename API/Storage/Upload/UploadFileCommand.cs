using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using KeepGrouped.API.AiBackend.Ingest;
using KeepGrouped.API.Rag;

namespace KeepGrouped.API.Storage;

public sealed class UploadFileCommand(IStorage storage, KeepGroupedDb db, RagQueue rag) : IHandler
{
	public async Task<Result<StorageFile>> ExecuteAsync(Stream content, string name, string contentType, long length, User creator, CancellationToken ct = default)
	{
		var res = await storage.UploadAsync(content, contentType, ct);
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
			Creator = creator,
		};

		db.Files.Add(filedb);
		if (name != creator.Id + "_avatar")
		{
			rag.Upsert(filedb);
		}
		await db.SaveChangesAsync(ct);
		return filedb;
	}
}

using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.AiBackend.Delete;

namespace KeepGrouped.API.Storage;

public sealed class DeleteFileCommand(IStorage storage, DeleteDocumentCommand command, KeepGroupedDb db) : IHandler
{
	public async Task<Result> ExecuteAsync(string key, CancellationToken cancellationToken = default)
	{
		var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
		if (filedb is null)
		{
			return StorageProblems.FileNotFound(key);
		}
		await command.ExecuteAsync(key, cancellationToken);

		var res = await storage.DeleteAsync(key);
		if ((int)res.Code >= 400)
		{
			return StorageProblems.DeleteFailed((int)res.Code);
		}

		db.Files.Remove(filedb);
		await db.SaveChangesAsync();
		return Result.OK;
	}
}

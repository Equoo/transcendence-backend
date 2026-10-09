using KeepGrouped.API;
using KeepGrouped.API.AiBackend.AiClient;
using KeepGrouped.API.AiBackend.Delete;
using KeepGrouped.API.AiBackend.Ingest;
using KeepGrouped.API.Storage;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Rag;

public sealed class RagFile(KeepGroupedDb db, ApiClient client, IStorage storage) : RagHandler
{
    public override string Kind => RagItem.FileKey;

    public override async ValueTask Delete(string id, CancellationToken ct = default)
    {
        await client.DeleteFileAsync<DeleteRagResponse>($"documents/{id}", ct);
    }

    public override async ValueTask Upsert(string id, CancellationToken ct = default)
    {
        await Delete(id, ct);

        var filedb = await db.Files.SingleOrDefaultAsync((fi) => fi.Key == id, ct);

        if (filedb is null)
        {
            return;
        }
        var fileContent = await storage.DownloadAsync(id, ct);
        if (((int)fileContent.Code) >= 400)
        {
            return;
        }

        await client.PostFileAsync<CreateRagResponse>($"documents/{id}", fileContent.Stream, filedb.Name, ct);
    }
}
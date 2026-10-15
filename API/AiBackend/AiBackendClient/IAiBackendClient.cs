using System.Net.ServerSentEvents;
using KeepGrouped.API.AiBackend.Ingest;
using KeepGrouped.API.Storage;

namespace KeepGrouped.API.AiBackend.AiClient;

public interface IAiBackendClient
{
	Task<CreateRagResponse> IngestFileAsync(Stream fileStream, string documentId, string fileName, CancellationToken cancellationToken = default);
	Task<KeepGrouped.API.AiBackend.Delete.DeleteRagResponse> DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default);
}


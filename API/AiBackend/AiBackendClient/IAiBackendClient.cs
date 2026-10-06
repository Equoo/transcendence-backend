using System.Net.ServerSentEvents;
using KeepGrouped.API.AiBackend.Ingest;

namespace KeepGrouped.API.AiBackend.AiClient;

public interface IAiBackendClient
{
	Task<IngestResponse> IngestFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}


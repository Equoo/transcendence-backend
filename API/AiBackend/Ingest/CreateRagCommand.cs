using KeepGrouped.API.Problems;
using KeepGrouped.API.AiBackend.AiClient;

namespace KeepGrouped.API.AiBackend.Ingest;

public sealed class CreateRagCommand(IAiBackendClient aiBackendClient)
{

	public async Task<Result<CreateRagResponse>> ExecuteAsync(Stream content, string fileName, string documentId, long length, CancellationToken cancellationToken = default)
	{
		if (length <= 0)
		{
			return AiBackendProblems.EmptyFile();
		}

		try
		{
			var response = await aiBackendClient.IngestFileAsync(content, documentId, fileName, cancellationToken);
			return response;
		}
		catch (HttpRequestException)
		{
			return AiBackendProblems.UpstreamUnavailable();
		}
	}
}
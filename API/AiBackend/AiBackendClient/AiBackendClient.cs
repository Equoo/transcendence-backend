using KeepGrouped.API.AiBackend.Ingest;
using KeepGrouped.API.AiBackend.Delete;

namespace KeepGrouped.API.AiBackend.AiClient;

public class AiBackendClient : IAiBackendClient
{
	private readonly IApiClient _apiClient;

	public AiBackendClient(IApiClient apiClient)
	{
		_apiClient = apiClient;
	}

	public Task<CreateRagResponse> IngestFileAsync(Stream fileStream, string documentId, string fileName, CancellationToken cancellationToken = default)
	=> _apiClient.PostFileAsync<CreateRagResponse>($"documents/{documentId}", fileStream, fileName, cancellationToken);

	public Task<DeleteRagResponse> DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default)
	=> _apiClient.DeleteFileAsync<DeleteRagResponse>($"documents/{documentId}", cancellationToken);
}
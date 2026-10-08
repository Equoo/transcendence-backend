using KeepGrouped.API.AiBackend.Chatbot;
using KeepGrouped.API.AiBackend.Ingest;
using System.Net.ServerSentEvents;
using KeepGrouped.API.AiBackend.Delete;
using System.Runtime.CompilerServices;

namespace KeepGrouped.API.AiBackend.AiClient;

public class AiBackendClient : IAiBackendClient
{
	private readonly IApiClient _apiClient;

	public AiBackendClient(IApiClient apiClient)
	{
		_apiClient = apiClient;
	}

	public Task<IngestResponse> IngestFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
	=> _apiClient.PostFileAsync<IngestResponse>("/ingest", fileStream, fileName, cancellationToken);

	public Task<DeleteResponse> DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default)
	=> _apiClient.DeleteFileAsync<DeleteResponse>($"documents/{documentId}", cancellationToken);
}
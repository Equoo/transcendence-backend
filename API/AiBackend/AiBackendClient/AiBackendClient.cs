
using System.Runtime.CompilerServices;

namespace KeepGrouped.API.AiBackend;

public class AiBackendClient : IAiBackendClient
{
	private readonly IApiClient _apiClient;

	public AiBackendClient(IApiClient apiClient)
	{
		_apiClient = apiClient;
	}

	public async IAsyncEnumerable<string> StreamChatAsync(
		string userId,
		string message,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new AiChatRequest
		{
			UserId = userId,
			Message = message
		};

		await foreach (var response in _apiClient.StreamAsync("/chat/stream", request, cancellationToken))
		{
			yield return response;
		}
	}

	public Task<IngestResponse> IngestFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
	=> _apiClient.PostFileAsync<IngestResponse>("/ingest", fileStream, fileName, cancellationToken);
}
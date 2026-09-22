
using System.Runtime.CompilerServices;

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
		var request = new ChatRequest
		{
			// UserId = userId,
			Message = message
		};

		await foreach (var response in _apiClient.StreamAsync("/chat/stream", request, cancellationToken))
		{
			yield return response;
		}
	}
}
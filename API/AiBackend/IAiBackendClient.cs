public interface IAiBackendClient
{
	IAsyncEnumerable<string> StreamChatAsync(
		string userId,
		string message,
		CancellationToken cancellationToken = default);
}


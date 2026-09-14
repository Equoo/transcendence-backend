using KeepGrouped.API;

public interface IAiBackendClient
{
	IAsyncEnumerable<string> StreamChatAsync(
		string sessionId,
		string message,
		CancellationToken cancellationToken = default);
}


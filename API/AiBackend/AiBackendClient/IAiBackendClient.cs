public interface IAiBackendClient
{
	IAsyncEnumerable<string> StreamChatAsync(
		string userId,
		string message,
		CancellationToken cancellationToken = default);

	Task<IngestResponse> IngestFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}


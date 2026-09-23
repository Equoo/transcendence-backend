
namespace KeepGrouped.API.AiBackend.AiClient;

public interface IApiClient
{
	IAsyncEnumerable<string> StreamAsync(string endpoint, object request, CancellationToken cancellationToken = default);
	Task<TResponse> PostFileAsync<TResponse>(string endpoint, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}


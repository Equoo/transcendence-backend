
using System.Net.ServerSentEvents;

namespace KeepGrouped.API.AiBackend.AiClient;

public interface IApiClient
{
	Task<TResponse> PostFileAsync<TResponse>(string endpoint, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
	Task<TResponse> DeleteFileAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
}


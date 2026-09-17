
public interface IApiClient
{
	Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
	Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
	IAsyncEnumerable<string> StreamAsync(string endpoint, object request, CancellationToken cancellationToken = default);
}


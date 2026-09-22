
public interface IApiClient
{
	IAsyncEnumerable<string> StreamAsync(string endpoint, object request, CancellationToken cancellationToken = default);
}


using System.Runtime.CompilerServices;

public class ApiClient : IApiClient
{
	private readonly HttpClient _httpClient;

	public ApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async IAsyncEnumerable<string> StreamAsync(string endpoint, object request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, endpoint)
		{
			Content = JsonContent.Create(request)
		}, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
		using var reader = new StreamReader(stream);
		string? line;
		string? current = null;
		while ((line = await reader.ReadLineAsync()) is not null)
		{
			if (line.StartsWith("data: "))
			{
				var data = line.Substring(6);
				if (current == "error")
					throw new InvalidOperationException(data);
				else
					yield return data;

			}
			else if (line.StartsWith("event: "))
			{
				current = line.Substring(7);
			}
			else if (line == string.Empty)
				current = null;
		}
	}

}
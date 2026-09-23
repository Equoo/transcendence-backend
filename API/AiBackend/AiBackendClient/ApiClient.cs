using System.Runtime.CompilerServices;
namespace KeepGrouped.API.AiBackend;

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

	public async Task<TResponse> PostFileAsync<TResponse>(string endpoint, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
	{
		using var content = new MultipartFormDataContent();
		using var streamContent = new StreamContent(fileStream);
		content.Add(streamContent, "file", fileName);

		var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
			?? throw new InvalidOperationException("Réponse vide du backend IA");
	}
}
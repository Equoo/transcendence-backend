using System.Runtime.CompilerServices;

namespace KeepGrouped.API.AiBackend.AiClient;

public class ApiClient(HttpClient httpClient) : IApiClient
{

	public async IAsyncEnumerable<string> StreamAsync(string endpoint, object request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, endpoint)
		{
			Content = JsonContent.Create(request)
		}, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
		using var reader = new StreamReader(stream);
		string? line;
		string? current = null;
		while ((line = await reader.ReadLineAsync()) is not null)
		{
			yield return line;
		}
	}

	public async Task<TResponse> PostFileAsync<TResponse>(string endpoint, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
	{
		using var content = new MultipartFormDataContent();
		using var streamContent = new StreamContent(fileStream);
		content.Add(streamContent, "file", fileName);

		var response = await httpClient.PostAsync(endpoint, content, cancellationToken);
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
			?? throw new InvalidOperationException("Réponse vide du backend IA");
	}
}
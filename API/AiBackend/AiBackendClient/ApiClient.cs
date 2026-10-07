using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;

namespace KeepGrouped.API.AiBackend.AiClient;

public class ApiClient(HttpClient httpClient) : IApiClient
{
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

	public async Task<TResponse> DeleteFileAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.DeleteAsync(endpoint, cancellationToken);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Response content is null");
	}
}
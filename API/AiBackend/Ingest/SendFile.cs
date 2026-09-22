using KeepGrouped.API.Problems;

namespace KeepGrouped.API.AiBackend;

public sealed class SendFile(IAiBackendClient aiBackendClient) : IHandler
{
	private static readonly string[] AllowedExtensions = [".txt", ".md", ".pdf"];

	public async Task<Result<IngestResponse>> ExecuteAsync(Stream content, string fileName, long length, CancellationToken cancellationToken)
	{
		if (length <= 0)
		{
			return AiBackendProblems.EmptyFile();
		}

		var extension = Path.GetExtension(fileName).ToLowerInvariant();
		if (!AllowedExtensions.Contains(extension))
		{
			return AiBackendProblems.UnsupportedFormat(extension);
		}

		try
		{
			var response = await aiBackendClient.IngestFileAsync(content, fileName, cancellationToken);
			return response;
		}
		catch (HttpRequestException)
		{
			return AiBackendProblems.UpstreamUnavailable();
		}
	}
}
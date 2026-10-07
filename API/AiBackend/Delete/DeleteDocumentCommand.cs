using KeepGrouped.API.AiBackend.AiClient;

namespace KeepGrouped.API.AiBackend.Delete;

public sealed class DeleteDocumentCommand(IAiBackendClient aiBackendClient) : IHandler
{
	public async Task<Result<DeleteResponse>> ExecuteAsync(string documentId, CancellationToken cancellationToken)
	{
		var response = await aiBackendClient.DeleteDocumentAsync(documentId, cancellationToken);
		return response;
	}
}
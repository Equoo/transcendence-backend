using KeepGrouped.API.AiBackend.AiClient;

namespace KeepGrouped.API.AiBackend.Delete;

public sealed class DeleteRagCommand(IAiBackendClient aiBackendClient)
{
	public async Task<Result<DeleteRagResponse>> ExecuteAsync(string documentId, CancellationToken cancellationToken = default)
	{
		var response = await aiBackendClient.DeleteDocumentAsync(documentId, cancellationToken);
		return response;
	}
}
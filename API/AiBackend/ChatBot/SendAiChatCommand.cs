using KeepGrouped.API.Middlewares;

namespace KeepGrouped.API.AiBackend;

public sealed partial class SendAiChatCommand(IAiBackendClient aiBackendClient, TokenContext tk) : IHandler
{
	public Result<IAsyncEnumerable<string>> ExecuteAsync(ChatRequest req, CancellationToken cancellationToken)
	{
		var stream = aiBackendClient.StreamChatAsync(tk.User.Id, req.Message, cancellationToken);

		return new Result<IAsyncEnumerable<string>>(stream);
	}
}
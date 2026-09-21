using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.AiBackend;

public sealed partial class SendAiChatCommand(IAiBackendClient aiBackendClient, TokenContext tk) : IHandler
{
	public Result<IAsyncEnumerable<string>> ExecuteAsync(ChatRequest req, User sender, CancellationToken cancellationToken)
	{
		var stream = aiBackendClient.StreamChatAsync(sender.Id, req.Message, cancellationToken);

		return new Result<IAsyncEnumerable<string>>(stream);
	}
}
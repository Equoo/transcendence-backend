using KeepGrouped.API.Middlewares;
using KeepGrouped.API.AiBackend.AiClient;

namespace KeepGrouped.API.AiBackend.Chatbot;

public sealed partial class SendAiChatCommand(IAiBackendClient aiBackendClient, TokenContext tk) : IHandler
{
	public Result<IAsyncEnumerable<string>> ExecuteAsync(ChatRequest req, CancellationToken cancellationToken)
	{
		var stream = aiBackendClient.StreamChatAsync(tk.User.Id, req.Message, cancellationToken);

		return new Result<IAsyncEnumerable<string>>(stream);
	}
}
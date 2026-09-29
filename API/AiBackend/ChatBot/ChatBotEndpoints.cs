using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.AiBackend.Chatbot;

public static class ChatBotEndpoints
{
	public static void MapChatBot(this IEndpointRouteBuilder aibackend)
	{
		var chatBot = aibackend.MapGroup("/chatbot").WithTags("ChatBot");

		chatBot.MapPost("/stream", [Authorize] async (SendAiChatCommand command, ChatRequest req, CancellationToken cancellationToken) =>
		{
			var result = command.ExecuteAsync(req, cancellationToken);
			if (result.IsProblem)
			{
				return (IResult)result.Problem;
			}

			return Results.ServerSentEvents(result.Value);
		})
		.RequireRateLimiting("ai-chat");

	}
}
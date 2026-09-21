using Microsoft.AspNetCore.Authorization;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.AiBackend;


public static class ChatBotEndpoints
{
	public static void MapChatBot(this IEndpointRouteBuilder aibackend)
	{
		var chatBot = aibackend.MapGroup("/chatbot").WithTags("ChatBot");

		chatBot.MapPost("/stream", [Authorize] async (SendAiChatCommand command, ChatRequest req, CancellationToken cancellationToken, HttpContext http) =>
		{
			var result = command.ExecuteAsync(req, cancellationToken);
			if (result.IsProblem)
			{
				return (IResult)result.Problem;
			}

			http.Response.ContentType = "text/event-stream";
			await foreach (var chunk in result.Value.WithCancellation(cancellationToken))
			{
				await http.Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
				await http.Response.Body.FlushAsync(cancellationToken);
			}
			return Results.Empty;
		})
		.RequireRateLimiting("ai-chat");

	}
}
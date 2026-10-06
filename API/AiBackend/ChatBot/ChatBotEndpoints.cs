using System.Net.Mime;
using System.Net.ServerSentEvents;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.AiBackend.Chatbot;

public class ChatRequest
{
	public string Message { get; set; } = null!;
}

public record AiChatRequest
{
	public string UserId { get; set; } = null!;
	public string Message { get; set; } = null!;
}

public static class ChatBotEndpoints
{
	public static void MapChatBot(this IEndpointRouteBuilder aibackend)
	{
		var chatBot = aibackend.MapGroup("/chatbot").WithTags("ChatBot");

		chatBot.MapPost("/stream", [Authorize] async (ChatRequest req, CancellationToken cancellationToken, HttpClient client, TokenContext tk, HttpContext ctx) =>
		{
			var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://ai-back-dev:7070/chat/stream")
			{
				Content = JsonContent.Create(new AiChatRequest() { Message = req.Message, UserId = tk.User.Id })
			}, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

			ctx.Response.ContentType = "text/event-stream";
			ctx.Response.Headers["X-Access-Buffering"] = "no";

			await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			await stream.CopyToAsync(ctx.Response.Body, cancellationToken);
			return Results.Empty;
		})
		.RequireRateLimiting("ai-chat");

	}
}
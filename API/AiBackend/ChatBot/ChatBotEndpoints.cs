using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;
using KeepGrouped.API.Problems;

namespace KeepGrouped.API.AiBackend.Chatbot;

public class ChatRequest
{
	public string Message { get; set; } = null!;
}

public static class ChatBotEndpoints
{
	public static void MapChatBot(this IEndpointRouteBuilder aibackend)
	{
		var chatBot = aibackend.MapGroup("/chatbot").WithTags("ChatBot");

		chatBot.MapPost("/stream", [Authorize] async (ChatRequest req, CancellationToken cancellationToken, HttpClient client, HttpContext ctx) =>
		{
			var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://ai-back-dev:7070/chat/stream")
			{
				Content = JsonContent.Create(req)
			}, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

			// if (!response.IsSuccessStatusCode)
			// {
			// 	response.Dispose();
			// 	return AiBackendProblems.ServiceUnavailable();
			// }

			ctx.Response.ContentType = "text/event-stream";
			ctx.Response.Headers["X-Access-Buffering"] = "no";


			await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			await stream.CopyToAsync(ctx.Response.Body, cancellationToken);
			return Results.Empty;
		})
		.RequireRateLimiting("ai-chat");

	}
}
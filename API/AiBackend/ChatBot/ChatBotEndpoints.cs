using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.AiBackend.Chatbot;

public class ChatRequest
{
	public string Message { get; set; } = null!;
}

public static class ChatBotEndpoints
{
	public static void MapChatBot(this IEndpointRouteBuilder aibackend)
	{
		var chatBot = aibackend.MapGroup("/chatbot");

		chatBot.MapPost("/stream", [Authorize] async Task<IResult> (
			ChatRequest req,
			CancellationToken cancellationToken,
			IHttpClientFactory factory,
			HttpContext ctx) =>
		{
			var client = factory.CreateClient("ai-back");
			HttpResponseMessage response;

			try
			{
				response = await client.SendAsync(
					new HttpRequestMessage(HttpMethod.Post, "chat/stream")
					{
						Content = JsonContent.Create(req)
					},
					HttpCompletionOption.ResponseHeadersRead,
					cancellationToken);
			}
			catch (HttpRequestException)
			{
				return AiBackendProblems.ServiceUnavailable();
			}

			if (!response.IsSuccessStatusCode)
				return AiBackendProblems.ServiceUnavailable();

			ctx.Response.ContentType = "text/event-stream";
			ctx.Response.Headers.CacheControl = "no-cache";
			ctx.Response.Headers["X-Accel-Buffering"] = "no";
			await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			await stream.CopyToAsync(ctx.Response.Body, cancellationToken);

			return Results.Empty;
		})
		.RequireRateLimiting("ai-chat");
	}
}
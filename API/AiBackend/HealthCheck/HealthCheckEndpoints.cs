using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.AiBackend.HealthCheck;

public static class HealthCheckEndpoints
{
	public static void MapHealthCheck(this IEndpointRouteBuilder aibackend)
	{
		var HealthCheck = aibackend.MapGroup("/health");

		HealthCheck.MapGet("/check", [Authorize] async (HttpClient client, CancellationToken cancellationToken) =>
		{

			try
			{
				using var response = await client.GetAsync("http://ai-back-dev:7070/health");

				return response.IsSuccessStatusCode
					? Results.Ok(new { status = "healthy" })
					: Results.Problem("Ai backend unavailable.", statusCode: StatusCodes.Status503ServiceUnavailable);
			}
			catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException && !cancellationToken.IsCancellationRequested)
			{
				return Results.Problem("Ai backend unavailable.", statusCode: StatusCodes.Status503ServiceUnavailable);
			}
		})
		.WithName("ai.health")
		.WithSummary("Check that the AI backend is available")
		.Produces(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status503ServiceUnavailable);
	}
}
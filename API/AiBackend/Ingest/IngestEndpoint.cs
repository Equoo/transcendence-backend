using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.AiBackend;

public static class Ingestpoints
{
	public static void MapIngest(this IEndpointRouteBuilder aibackend)
	{
		var ingest = aibackend.MapGroup("/ingest").WithTags("Ingest");

		ingest.MapPost("/", [Authorize] async (SendFile command, [FromForm] IngestRequest req, CancellationToken cancellationToken) =>
		{
			var result = await command.ExecuteAsync(
				req.File.OpenReadStream(), req.File.FileName, req.File.Length, cancellationToken);

			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.RequireRateLimiting("ai-ingest")
		.DisableAntiforgery()
		.WithName("ai.ingest")
		.WithSummary("Ingère un document dans la base de connaissances RAG")
		.WithDescription("Envoie un fichier en `multipart/form-data` (champ `file`) pour être découpé, vectorisé et stocké pour le retrieval.")
		.Accepts<IngestRequest>("multipart/form-data")
		.Produces<IngestResponse>(StatusCodes.Status200OK)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status502BadGateway)
		.RequireRateLimiting("ai-chat");
	}
}
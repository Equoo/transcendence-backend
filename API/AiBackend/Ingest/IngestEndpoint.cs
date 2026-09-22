using KeepGrouped.API.AiBackend;
using Microsoft.AspNetCore.Authorization;
using KeepGrouped.API.Middlewares;

public static class Ingestpoints
{
	public static void MapIngest(this IEndpointRouteBuilder aibackend)
	{
		var ingest = aibackend.MapGroup("/ingest").WithTags("Ingest");

		// MapIngest.MapPost("", [Authorize] async ());
	}
}
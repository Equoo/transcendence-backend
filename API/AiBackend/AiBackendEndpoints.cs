namespace KeepGrouped.API.AiBackend;

public static class AiBackendEndpoints
{
	public static void MapAiBackend(this IEndpointRouteBuilder app)
	{
		var aibackend = app.MapGroup("/ai").WithTags("AiBackend");

		aibackend.MapChatBot();
	}
}

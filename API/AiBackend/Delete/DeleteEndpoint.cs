
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.AiBackend.Delete;

public static class DeleteEndpoint
{
	public static void MapDeleteFileEndpoint(this IEndpointRouteBuilder aibackend)
	{
		var documents = aibackend.MapGroup("/documents");

		documents.MapDelete("/{id}", [Authorize] async (string id, DeleteDocumentCommand command, CancellationToken cancellationToken) =>
		{
			var result = await command.ExecuteAsync(id, cancellationToken);
			if (result.IsProblem)
			{
				return (IResult)result.Problem;
			}
			return Results.Ok(result.Value);
		});
	}
}
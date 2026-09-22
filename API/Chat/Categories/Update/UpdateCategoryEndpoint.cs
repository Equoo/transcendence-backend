using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public record UpdateCategoryRequest
{
	[Required]
	[Length(1, 25)]
	public string Name { get; init; } = null!;
	public uint Order { get; init; } = 0;
}

public static class UpdateCategoryEndpoint
{
	public static void MapUpdateCategory(this IEndpointRouteBuilder categories)
	{
		categories.MapPut("/{id}", [Authorize][Roles(Perms.HandleChannels)] async (UpdateCategoryCommand command, string id, UpdateCategoryRequest req, TokenContext token) =>
		{
			var result = await command.ExecuteAsync(id, req, token.User);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.NoContent();
		})
		.WithName("categories.update")
		.WithSummary("Update a channel category")
		.WithDescription("Replaces the name and the display order of a channel category. The name must stay unique. Online users are notified of the update.")
		.Produces(StatusCodes.Status204NoContent)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status409Conflict)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

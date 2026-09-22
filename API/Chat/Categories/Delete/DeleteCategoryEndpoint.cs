using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class DeleteCategoryEndpoint
{
	public static void MapDeleteCategory(this IEndpointRouteBuilder categories)
	{
		categories.MapDelete("/{id}", [Authorize][Roles(Perms.HandleChannels)] async (DeleteCategoryCommand command, string id, TokenContext token) =>
		{
			var result = await command.ExecuteAsync(id, token.User);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.NoContent();
		})
		.WithName("categories.delete")
		.WithSummary("Delete a channel category")
		.WithDescription("Deletes a channel category. Channels it contains are left without a category. Online users are notified of the removal.")
		.Produces(StatusCodes.Status204NoContent)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

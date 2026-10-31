using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Channels;

public record ReorderCategoriesRequest
{
	[Required]
	public List<string> Categories { get; init; } = [];
}

public static class ReorderCategoriesEndpoint
{
	public static void MapReorderCategories(this IEndpointRouteBuilder categories)
	{
		categories.MapPut("/order", [Authorize][Roles(Perms.HandleChannels)] async (ReorderCategoriesCommand command, ReorderCategoriesRequest req) =>
		{
			var result = await command.ExecuteAsync(req);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("categories.reorder")
		.WithSummary("Reorder channel categories")
		.WithDescription("Sets the display order of every channel category. `categories` must list each category id exactly once, in the new order. Online users are notified of each updated category.")
		.Produces<IEnumerable<ChannelCategoryResponse>>(StatusCodes.Status200OK)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

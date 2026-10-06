using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class ListCategoriesEndpoint
{
	public static void MapListCategories(this IEndpointRouteBuilder categories)
	{
		categories.MapGet("/", [Authorize] async (ListCategoriesQuery query) =>
		{
			var result = await query.ExecuteAsync();
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("categories.list")
		.WithSummary("List channel categories")
		.WithDescription("Returns every channel category, ordered by their display order.")
		.Produces<IEnumerable<ChannelCategoryResponse>>(StatusCodes.Status200OK);
	}
}

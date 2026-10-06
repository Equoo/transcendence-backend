using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class GetCategoryEndpoint
{
	public static void MapGetCategory(this IEndpointRouteBuilder categories)
	{
		categories.MapGet("/{id}", [Authorize] async (GetCategoryQuery query, string id) =>
		{
			var result = await query.ExecuteAsync(id);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("categories.get")
		.WithSummary("Get a channel category")
		.WithDescription("Returns a single channel category identified by its id.")
		.Produces<ChannelCategoryResponse>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

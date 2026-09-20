using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public record CreateCategoryRequest
{
	[Required]
	[Length(1, 25)]
	public string Name { get; init; } = null!;
	public uint Order { get; init; } = 0;
}

public static class CreateCategoryEndpoint
{
	public static void MapCreateCategory(this IEndpointRouteBuilder categories)
	{
		categories.MapPost("/", [Authorize][Roles((int)Perms.HandleChannels)] async (CreateCategoryCommand command, CreateCategoryRequest req, TokenContext token) =>
		{
			var result = await command.ExecuteAsync(req, token.User);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.CreatedAtRoute("categories.get", new { id = result.Value.Id }, result.Value);
		})
		.WithName("categories.create")
		.WithSummary("Create a channel category")
		.WithDescription("Creates a new channel category. The name must be unique and free of special characters. Online users are notified of the new category.")
		.Produces<ChannelCategoryResponse>(StatusCodes.Status201Created)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status409Conflict)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

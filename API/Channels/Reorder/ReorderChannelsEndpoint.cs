using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Channels;

public record ReorderChannelsRequest
{
	public string? Category { get; init; } = null;
	[Required]
	public List<string> Channels { get; init; } = [];
}

public static class ReorderChannelsEndpoint
{
	public static void MapReorderChannels(this IEndpointRouteBuilder channels)
	{
		channels.MapPut("/order", [Authorize][Roles(Perms.HandleChannels)] async (ReorderChannelsCommand command, ReorderChannelsRequest req) =>
		{
			var result = await command.ExecuteAsync(req);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("channels.reorder")
		.WithSummary("Reorder the channels of a category")
		.WithDescription("Sets the display order of the text channels of `category` (`null` for channels without category). `channels` must list each channel already in the category exactly once, in the new order. Channels listed from another category are moved into it. Online users are notified of each updated channel.")
		.Produces<IEnumerable<ChannelResponse>>(StatusCodes.Status200OK)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

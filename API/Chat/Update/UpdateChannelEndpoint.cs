using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public record UpdateChannelRequest
{
	[Required]
	[Length(1, 25)]
	public string Name { get; init; } = null!;
	[Length(0, 255)]
	public string Topic { get; init; } = string.Empty;
	public string? Category { get; init; } = null;
}

public static class UpdateChannelEndpoint
{
	public static void MapUpdateChannel(this IEndpointRouteBuilder channels)
	{
		channels.MapPut("/{id}", [Authorize][Roles((int)Perms.HandleChannels)] async (UpdateChannelCommand command, string id, UpdateChannelRequest req, TokenContext token) =>
		{
			var result = await command.ExecuteAsync(id, req, token.User);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.NoContent();
		})
		.WithName("channels.update")
		.WithSummary("Update a channel")
		.WithDescription("Replaces the name, the topic and the category of a channel. A null category detaches the channel from its current one. The event a channel belongs to cannot be changed. Online users are notified of the update.")
		.Produces(StatusCodes.Status204NoContent)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status409Conflict)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

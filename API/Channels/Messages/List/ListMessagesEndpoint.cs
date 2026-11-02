using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Channels;

public static class ListMessagesEndpoint
{
	public static void MapListMessages(this IEndpointRouteBuilder messages)
	{
		messages.MapGet("/", [Authorize] async (ListMessagesQuery query, string id, TokenContext token, DateTime? before, DateTime? after, DateTime? around, [Range(1, 100)] int take = 20) =>
		{
			var result = await query.ExecuteAsync(id, before, after, around, take, token.User);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("messages.list")
		.WithSummary("List the messages of a channel")
		.WithDescription("Returns the last messages of the channel in chronological order. Can be limited with `take` (1 to 100, 20 by default) and paged backwards with `before`.")
		.Produces<IEnumerable<MessageResponse>>(StatusCodes.Status200OK)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

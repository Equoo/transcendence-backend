using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Conversations;

public record CreateConversationRequest
{
	[Required]
	[Length(1, 9)]
	public List<string> Recipients { get; init; } = [];
	[Length(0, 25)]
	public string? Name { get; init; } = null;
}

public static class CreateConversationEndpoint
{
	public static void MapCreateConversation(this IEndpointRouteBuilder conversations)
	{
		conversations.MapPost("/", [Authorize] async (CreateConversationCommand command, CreateConversationRequest req, TokenContext tk) =>
		{
			var result = await command.ExecuteAsync(tk.User, req);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.CreatedAtRoute("me.conversations.get", new { id = result.Value.Id }, result.Value);
		})
		.WithName("me.conversations.create")
		.WithSummary("Start a conversation")
		.WithDescription("Starts a direct message with a single recipient, or a group direct message with several. A direct message that already exists with the recipient is returned instead of a new one. Members are notified of the new conversation.")
		.Produces<ConversationResponse>(StatusCodes.Status201Created)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status403Forbidden)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

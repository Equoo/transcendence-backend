using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Conversations;

public static class DeleteConversationEndpoint
{
	public static void MapDeleteConversation(this IEndpointRouteBuilder conversations)
	{
		conversations.MapDelete("/{id}", [Authorize] async (DeleteConversationCommand command, string id, TokenContext tk) =>
		{
			var result = await command.ExecuteAsync(tk.User.Id, id);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.NoContent();
		})
		.WithName("me.conversations.delete")
		.WithSummary("Leave a group conversation")
		.WithDescription("Removes the current user from a group direct message. The conversation is deleted once its last member leaves. Direct messages cannot be left. Remaining members are notified.")
		.Produces(StatusCodes.Status204NoContent)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status422UnprocessableEntity);
	}
}

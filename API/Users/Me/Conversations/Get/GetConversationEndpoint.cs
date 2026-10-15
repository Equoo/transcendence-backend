using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Conversations;

public static class GetConversationEndpoint
{
	public static void MapGetConversation(this IEndpointRouteBuilder conversations)
	{
		conversations.MapGet("/{id}", [Authorize] async (GetConversationQuery query, string id, TokenContext tk) =>
		{
			var result = await query.ExecuteAsync(tk.User.Id, id);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("me.conversations.get")
		.WithSummary("Get a conversation")
		.WithDescription("Returns a single conversation of the current user, identified by its id.")
		.Produces<ConversationResponse>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

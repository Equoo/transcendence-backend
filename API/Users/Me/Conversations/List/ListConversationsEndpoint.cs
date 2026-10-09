using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Conversations;

public static class ListConversationsEndpoint
{
	public static void MapListConversations(this IEndpointRouteBuilder conversations)
	{
		conversations.MapGet("/", [Authorize] async (ListConversationsQuery query, TokenContext tk) =>
		{
			var result = await query.ExecuteAsync(tk.User.Id);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("me.conversations.list")
		.WithSummary("List the current user's conversations")
		.WithDescription("Returns the direct messages and group direct messages the current user is a member of.")
		.Produces<IEnumerable<ConversationResponse>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status401Unauthorized);
	}
}

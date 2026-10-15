using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Conversations;

public static class ListConversationsEndpoint
{
	public static void MapListConversations(this IEndpointRouteBuilder conversations)
	{
		conversations.MapGet("/", [Authorize] async (ListConversationsQuery query, TokenContext tk, DateTime? before, int take = 20) =>
		{
			var result = await query.ExecuteAsync(tk.User.Id, before, take);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("me.conversations.list")
		.WithSummary("List the current user's conversations")
		.WithDescription("Returns the direct messages and group direct messages the current user is a member of, most recently active first. Can be limited with `take` and paged backwards with `before`, compared to the last message date (or the creation date when there is no message).")
		.Produces<IEnumerable<ConversationResponse>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status401Unauthorized);
	}
}

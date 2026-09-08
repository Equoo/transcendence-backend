using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public record GetMeResponse(string Id, string UserName, Dictionary<string, DateTime> ChannelsAckMsg)
{
	public static GetMeResponse FromEntity(User user)
	{
		var ChannelsAckMsg = new Dictionary<string, DateTime>();

		foreach (var ack in user.ChannelsAckMsg)
		{
			ChannelsAckMsg.Add(ack.ChannelId, ack.AckAt);
		}

		return new(user.Id, user.UserName, ChannelsAckMsg);
	}
}

public static class GetMeEndpoint
{
	// No handler: returning the current user touches neither the database nor the object store.
	public static void MapGetMe(this IEndpointRouteBuilder me)
	{
		me.MapGet(
				"/",
				[Authorize]
		(TokenContext token) =>
				{
					return Results.Ok(GetMeResponse.FromEntity(token.User));
				}
			)
			.WithName("me.get")
			.WithSummary("Get the current user")
			.WithDescription("Returns the profile of the user the access token belongs to.")
			.Produces<GetMeResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized);
	}
}

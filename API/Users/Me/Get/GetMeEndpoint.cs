using System.Security.Cryptography.X509Certificates;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Storage;
using KeepGrouped.Migrations;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;


public record GetMeResponse(string Id, string UserName, GetFileMetaResponse? Avatar, RoleResponse Role, LanguageEnum Language,
    Dictionary<string, DateTime> ChannelsAckMsg)
{
    public static GetMeResponse FromEntity(User user) => new(user.Id, user.UserName,
    user.Avatar != null ? GetFileMetaResponse.FromEntity(user.Avatar) : null, RoleResponse.FromEntity(user.Role),
    user.Language, user.ChannelsAckMsg.ToDictionary(ack => ack.ChannelId, ack => ack.AckAt));
}

public static class GetMeEndpoint
{
    // No handler: returning the current user touches neither the database nor the object store.
    public static void MapGetMe(this IEndpointRouteBuilder me)
    {
        me.MapGet(
                "/",
                [Authorize]
        async (TokenContext token, GetMeCommand cmd) =>
                {
                    var res = await cmd.ExecuteAsync();

                    if (res.IsProblem)
                    {
                        return res.Problem;
                    }

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

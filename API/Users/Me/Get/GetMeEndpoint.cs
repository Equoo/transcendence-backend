using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public record GetMeResponse(string Id, string UserName)
{
    public static GetMeResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class GetMeEndpoint
{
    // No handler: returning the current user touches neither the database nor the object store.
    public static void MapGetMe(this IEndpointRouteBuilder me)
    {
        me.MapGet("/", [Authorize] (TokenContext token) =>
        {
            return Results.Ok(GetMeResponse.FromEntity(token.User));
        })
        .WithName("me.get")
        .WithSummary("Get the current user")
        .WithDescription("Returns the profile of the user the access token belongs to.")
        .Produces<GetMeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

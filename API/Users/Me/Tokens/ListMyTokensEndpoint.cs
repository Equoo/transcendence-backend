using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

/// <summary>
/// One live session of the current user. <c>Id</c> is the SHA-256 of the refresh token, never the
/// token itself — it identifies the session without being usable to authenticate.
/// </summary>
public record ListMyTokensResponse(string Id)
{
    public static ListMyTokensResponse FromEntity(RefreshToken token) => new(token.Id);
}

public static class ListMyTokensEndpoint
{
    public static void MapListMyTokens(this IEndpointRouteBuilder me)
    {
        me.MapGet("/tokens", [Authorize] async (ListMyTokensQuery query, TokenContext tk) =>
        {
            var result = await query.ExecuteAsync(tk.User.Id);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("me.tokens.list")
        .WithSummary("List the current user's sessions")
        .WithDescription("Returns one entry per stored refresh token of the current user, that is one per device still logged in.")
        .Produces<IEnumerable<ListMyTokensResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

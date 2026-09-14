namespace KeepGrouped.API.Users;

/// <summary>Who the rotated session now belongs to.</summary>
public record RefreshResponse(string Id, string UserName)
{
    public static RefreshResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class RefreshEndpoint
{
    public static void MapRefresh(this IEndpointRouteBuilder auth)
    {

        auth.MapGet("/refresh", async (RefreshTokensCommand command, HttpContext http) =>
        {
            var result = await command.ExecuteAsync(TokenCookies.Get(http, "RefreshToken"));
            if (result.IsProblem)
            {
                // Every failure path of this route drops the cookies before answering.
                TokenCookies.Remove(http);
                return result.Problem;
            }

            TokenCookies.Write(http, result.Value.Tokens);

            return Results.Ok(result.Value.User);
        })
        .WithName("auth.refresh")
        .WithSummary("Rotate the session tokens")
        .WithDescription("Exchanges the `RefreshToken` cookie for a fresh token pair. The old refresh token is revoked, and both cookies are cleared on any failure.")
        .Produces<RefreshResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

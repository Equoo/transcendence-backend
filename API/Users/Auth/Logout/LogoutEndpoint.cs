using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public static class LogoutEndpoint
{
    // No handler: logging out only clears cookies, it touches no store.
    public static void MapLogout(this IEndpointRouteBuilder auth)
    {
        auth.MapPost("/logout", [Authorize] (HttpContext http) =>
        {
            TokenCookies.Remove(http);
            return Results.NoContent();
        })
        .WithName("auth.logout")
        .WithSummary("Log the current user out")
        .WithDescription("Clears the `AccessToken` and `RefreshToken` cookies. The stored refresh token is not revoked: use it from another device and it still works.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

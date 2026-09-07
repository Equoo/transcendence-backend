using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public static class LogoutUserEndpoint
{
    public static void MapLogoutUser(this IEndpointRouteBuilder auth)
    {
        auth.MapDelete("/logout/{id}", [Authorize] async (LogoutQuery query, HttpContext http, string id) =>
        {
            var result = await query.ExecAsync(id);

            return Results.NoContent();
        })
        .WithName("auth.logout.user")
        .WithSummary("Log the selected user out")
        .WithDescription("Clears the `AccessToken` and `RefreshToken` cookies. The stored refresh token is revoked: use it from another device and it still works.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

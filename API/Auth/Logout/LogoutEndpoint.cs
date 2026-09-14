using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public static class LogoutEndpoint
{
    public static void MapLogout(this IEndpointRouteBuilder auth)
    {
        auth.MapGet("/logout", [Authorize] async (LogoutQuery query, HttpContext http, TokenContext tk) =>
        {
            var result = await query.ExecAsync(tk.User.Id);
            
            if (result.IsProblem)
            {
                return result.Problem;
            }

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

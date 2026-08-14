using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public record DeleteMeResponse(string Id, string UserName)
{
    public static DeleteMeResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class DeleteMeEndpoint
{
    public static void MapDeleteMe(this IEndpointRouteBuilder me)
    {
        me.MapDelete("/", [Authorize] async (DeleteMeCommand command, HttpContext http, TokenContext tk) =>
        {
            var result = await command.ExecuteAsync(tk.User);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            TokenCookies.Remove(http);

            return Results.Ok(result.Value);
        })
        .WithName("me.delete")
        .WithSummary("Delete the current user")
        .WithDescription("Deletes the current user, revokes their refresh tokens and clears their cookies.")
        .Produces<DeleteMeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

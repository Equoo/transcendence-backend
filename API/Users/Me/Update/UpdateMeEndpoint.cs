using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

/// <summary>
/// Renaming is all this use case does, so it asks for a name and nothing else — a password sent
/// here used to be accepted and ignored.
/// </summary>
public record UpdateMeRequest
{
    [Required]
    [Length(1, 255)]
    public string UserName { get; init; } = null!;
}

public record UpdateMeResponse(string Id, string UserName)
{
    public static UpdateMeResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class UpdateMeEndpoint
{
    public static void MapUpdateMe(this IEndpointRouteBuilder me)
    {
        me.MapPut("/", [Authorize] async (UpdateMeCommand command, UpdateMeRequest req, TokenContext tk) =>
        {
            var result = await command.ExecuteAsync(tk.User, req);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("me.update")
        .WithSummary("Rename the current user")
        .WithDescription("Changes the current user's name. Names are unique across the API.")
        .Produces<UpdateMeResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

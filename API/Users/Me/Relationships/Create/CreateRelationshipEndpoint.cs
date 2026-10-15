using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class CreateRelationshipEndpoint
{
    public static void MapCreateRelationship(this IEndpointRouteBuilder relationships)
    {
        relationships.MapPost("/{userId}", [Authorize] async (CreateRelationshipCommand command, string userId, TokenContext tk) =>
        {
            var result = await command.ExecuteAsync(tk.User, userId);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("me.relationships.create")
        .WithSummary("Send or accept a friend request")
        .WithDescription("Sends a friend request to the user, or accepts it if that user already sent one to the current user.")
        .Produces<RelationshipResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }
}

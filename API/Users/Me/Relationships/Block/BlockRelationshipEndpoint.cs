using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class BlockRelationshipEndpoint
{
    public static void MapBlockRelationship(this IEndpointRouteBuilder relationships)
    {
        relationships.MapPost("/{userId}/block", [Authorize] async (BlockRelationshipCommand command, string userId, TokenContext tk) =>
        {
            var result = await command.ExecuteAsync(tk.User, userId);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("me.relationships.block")
        .WithSummary("Block a user")
        .WithDescription("Blocks the user, ending any friendship or pending friend request with them.")
        .Produces<RelationshipResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }
}

using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class DeleteRelationshipEndpoint
{
    public static void MapDeleteRelationship(this IEndpointRouteBuilder relationships)
    {
        relationships.MapDelete("/{userId}", [Authorize] async (DeleteRelationshipCommand command, string userId, TokenContext tk) =>
        {
            var result = await command.ExecuteAsync(tk.User, userId);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("me.relationships.delete")
        .WithSummary("Remove a relationship")
        .WithDescription("Removes a friend, cancels or declines a friend request, or unblocks the user.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

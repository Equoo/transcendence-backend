using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Relationships;

public record UpdateRelationshipRequest
{
    [Length(1, 255)]
    public string? Nickname { get; init; }
}

public static class UpdateRelationshipEndpoint
{
    public static void MapUpdateRelationship(this IEndpointRouteBuilder relationships)
    {
        relationships.MapPatch("/{userId}", [Authorize] async (UpdateRelationshipCommand command, string userId, UpdateRelationshipRequest req, TokenContext tk) =>
        {
            var result = await command.ExecuteAsync(tk.User, userId, req);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("me.relationships.update")
        .WithSummary("Rename a relationship")
        .WithDescription("Sets the nickname the current user gives to the user, or clears it when null. Only the current user sees it.")
        .Produces<RelationshipResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

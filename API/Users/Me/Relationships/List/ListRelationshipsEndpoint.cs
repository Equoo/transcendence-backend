using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class ListRelationshipsEndpoint
{
    public static void MapListRelationships(this IEndpointRouteBuilder relationships)
    {
        relationships.MapGet("/", [Authorize] async (ListRelationshipsQuery query, TokenContext tk) =>
        {
            var result = await query.ExecuteAsync(tk.User.Id);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("me.relationships.list")
        .WithSummary("List the current user's relationships")
        .WithDescription("Returns the friends, pending friend requests and blocked users of the current user.")
        .Produces<IEnumerable<RelationshipResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

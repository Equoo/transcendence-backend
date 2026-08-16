using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Invitation;

public static class ListInvitationEndpoint
{
    public static void MapListInvitation(this RouteGroupBuilder route)
    {
        route.MapGet("/", [Authorize] async (ListInvitationQuery query) =>
        {
            var result = await query.ExecuteAsync();

            if (result.Problem is { } problem)
            {
                return problem;
            }
            return Results.Ok(result.Value);
        })
        .WithName("invitations.list")
        .WithSummary("List the invitations")
        .WithDescription("Returns every invitation with its id, expiry date and remaining uses.")
        .Produces<ICollection<Invitation>>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError); ;
    }
}
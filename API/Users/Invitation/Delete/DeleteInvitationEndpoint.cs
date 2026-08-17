using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Invitation;

public static class DeleteInvitationEndpoint
{
    public static void MapDeleteInvitation(this RouteGroupBuilder route)
    {
        route.MapDelete("/{id}", [Authorize] async (DeleteInvitationCommand cmd, string id) =>
        {
            var result = await cmd.ExecuteAsync(id);

            if (result.Problem is { } problem)
            {
                return problem;
            }
            return Results.NoContent();
        })
        .WithName("invitations.delete")
        .WithSummary("Delete an invitation")
        .WithDescription("Deletes an invitation by its id, revoking the link immediately whatever its remaining uses or expiry date.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

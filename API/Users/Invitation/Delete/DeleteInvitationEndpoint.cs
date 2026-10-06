using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Invitation;

public static class DeleteInvitationEndpoint
{
	public static void MapDeleteInvitation(this RouteGroupBuilder route)
	{
		route.MapDelete("/{id}", [Authorize][Roles(Perms.InviteUser)] async (DeleteInvitationCommand cmd, string id) =>
		{
			var result = await cmd.ExecuteAsync(id);

			if (result.IsProblem)
			{
				return result.Problem;
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

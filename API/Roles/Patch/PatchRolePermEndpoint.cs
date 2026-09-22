
using KeepGrouped.API.Attributes.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class PatchRolePermEndpoint
{
	public static void MapPatchPermRole(this IEndpointRouteBuilder roles)
	{
		roles.MapPatch("/{id}/permission", [Authorize][Roles(Perms.HandleRoles)] async (PatchRolePermQuery query, string id, [FromBody] int permission) =>
		{
			var res = await query.ExecAsync(id, (Perms)permission);
			if (res.IsProblem)
			{
				return res.Problem;
			}

			return Results.Ok();
		});
	}
}

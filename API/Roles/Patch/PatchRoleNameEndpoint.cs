
using KeepGrouped.API.Attributes.Roles;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class PatchRoleNameEndpoint
{
	public static void MapPatchNameRole(this IEndpointRouteBuilder roles)
	{
		roles.MapPatch("/{id}/name", [Authorize][Roles(Perms.HandleRoles)] async (PatchRoleNameQuery query, string id, [FromBody][Required][Length(1, Role.NameMaxLength)][RegularExpression(Role.NamePattern)] string name) =>
		{
			var res = await query.ExecAsync(id, name);
			if (res.IsProblem)
			{
				return res.Problem;
			}

			return Results.Ok();
		});
	}
}

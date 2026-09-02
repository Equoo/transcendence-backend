
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class PatchRoleNameEndpoint
{
    public static void MapPatchNameRole(this IEndpointRouteBuilder roles)
    {
        roles.MapPatch("/{id}/name", [Authorize] async (PatchRoleNameQuery query, string id, [FromBody] string name) =>
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
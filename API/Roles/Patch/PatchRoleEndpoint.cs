
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class PatchRoleEndpoint
{
    public static void MapPatchRole(this IEndpointRouteBuilder roles)
    {
        roles.MapPatch("/{id}", [Authorize] async (PatchRoleQuery query, string id, [FromBody] int permission) =>
        {
            var res = await query.ExecAsync(id, permission);
            if (res.IsProblem)
            {
                return res.Problem;
            }
            
            return Results.Ok();
        });
    }
}
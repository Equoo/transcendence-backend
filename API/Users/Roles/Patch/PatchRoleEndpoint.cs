
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Roles;

public static class PatchRoleEndpoint
{
    public static void MapPatchRole(this IEndpointRouteBuilder users)
    {
        users.MapPatch("/{IdUser}/role/{IdRole}", [Authorize][Roles((Perms.HandleUsers | Perms.HandleRoles))] async (PatchRoleQuery query, string IdUser, string IdRole) =>
        {
            var result = await query.ExecuteAsync(IdUser, IdRole);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok();
        });
    }
}

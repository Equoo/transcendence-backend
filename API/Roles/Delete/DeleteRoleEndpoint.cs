using KeepGrouped.API.Attributes.Roles;
using Microsoft.AspNetCore.Authorization;
using Npgsql.Replication;

namespace KeepGrouped.API.Roles;

public static class DeleteRoleEndpoint
{
    public static void MapDeleteRole(this IEndpointRouteBuilder roles)
    {
        roles.MapDelete("/{id}", [Authorize][Roles(Perms.HandleRoles)] async (string id, DeleteRoleQuery query) =>
        {
            var result = await query.ExecAsync(id);
            if (result.IsProblem)
            {
                return result.Problem;
            }
            return Results.Ok();
        });
    }
}

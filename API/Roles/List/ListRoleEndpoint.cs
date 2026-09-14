
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Roles;

public record RoleResponse(string Id, string Name, int Permission)
{
    public static RoleResponse FromEntity(Role role) => new(role.Id, role.Name, role.Permission);
}

public static class ListRoleEndpoint
{
    public static void MapListRole(this IEndpointRouteBuilder roles)
    {
        roles.MapGet("/", [Authorize] async (ListRoleQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        });
    }
}

using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Roles;

public static class GetRoleEndpoint
{
    public static void MapGetRoles(this IEndpointRouteBuilder roles)
    {
        roles.MapGet("/", [Authorize] async (GetRoleQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result._value);
        });
    }
}
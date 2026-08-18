
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

            return Results.Ok(result._value);
        });
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class CreateRoleEndpoint
{
    public static void MapCreateRole(this IEndpointRouteBuilder roles)
    {
        roles.MapPut("/", [Authorize] async (CreateRoleQuery query, [FromBody] string name) =>
        {
            var res = await query.ExecAsync(name);
            
            if (res.IsProblem)
            {
                return res.Problem;
            }

            return Results.Ok();            
        });
    }
}
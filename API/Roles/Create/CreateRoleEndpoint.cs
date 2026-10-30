using KeepGrouped.API.Attributes.Roles;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class CreateRoleEndpoint
{
    public static void MapCreateRole(this IEndpointRouteBuilder roles)
    {
        roles.MapPost("/", [Authorize][Roles(Perms.HandleRoles)] async (CreateRoleCommand query, [FromBody][Required][Length(1, Role.NameMaxLength)][RegularExpression(Role.NamePattern)] string name) =>
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

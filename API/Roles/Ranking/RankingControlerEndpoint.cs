
using KeepGrouped.API.Attributes.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Roles;

public static class RankingUpEndpoint
{
    public static void MapRankingControl(this IEndpointRouteBuilder roles)
    {
        roles.MapPost("/{id}/rank/{direction}", [Authorize][Roles(Perms.HandleRoles)] async (RankingControlCommand cmd, string id, string direction) =>
        {
            var res = await cmd.ExecAsync(id, direction);
            if (res.IsProblem)
            {
                return res.Problem;
            }

            return Results.Ok();
        });
    }
}
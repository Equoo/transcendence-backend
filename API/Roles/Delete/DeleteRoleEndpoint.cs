using Npgsql.Replication;

namespace KeepGrouped.API.Roles;

public static class DeleteRoleEndpoint
{
    public static void MapDeleteRole(this IEndpointRouteBuilder roles)
    {
        roles.MapDelete("/{id}", async (string id, DeleteRoleQuery query) =>
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
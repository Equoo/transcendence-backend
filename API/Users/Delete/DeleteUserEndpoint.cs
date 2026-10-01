
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Users.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public static class DeleteUserEndpoint
{
    public static void MapDeleteUser(this IEndpointRouteBuilder users)
    {


        users.MapDelete("/{id}", [Authorize][Roles((int)Perms.HandleUsers)] async (DeleteUserQuery query, string id) =>
        {
            var result = await query.ExecuteAsync(id);
            if (result.IsProblem)
            {
                return result.Problem;
            }
            
            return Results.Ok();
        });
    }
}

using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public static class DeleteUserEndpoint
{
    public static void MapDeleteUser(this IEndpointRouteBuilder users)
    {

        users.MapDelete("/{id}", [Authorize] async (DeleteUserQuery query, string id) =>
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
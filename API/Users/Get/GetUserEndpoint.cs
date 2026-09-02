using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public record GetUserResponse(string Id, string UserName, RoleResponse Role)
{
    public static GetUserResponse FromEntity(User user) => new(user.Id, user.UserName, RoleResponse.FromEntity(user.Role));
}

public static class GetUserEndpoint
{
    public static void MapGetUser(this IEndpointRouteBuilder users)
    {
        users.MapGet("/{id}", [Authorize] async (GetUserQuery query, string id) =>
        {
            var result = await query.ExecuteAsync(id);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("users.get")
        .WithSummary("Get a user")
        .WithDescription("Returns the public profile of a single user identified by their id, with how many events they organize and how many they joined.")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

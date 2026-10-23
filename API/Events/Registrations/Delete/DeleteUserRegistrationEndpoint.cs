using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public static class DeleteUserRegistrationEndpoint
{
    public static void MapDeleteUserRegistration(this IEndpointRouteBuilder registrations)
    {
        registrations.MapDelete("/{userId}", [Authorize][Roles(Perms.HandleEvent)] async (DeleteUserRegistrationCommand command, string id, string userId) =>
        {
            var result = await command.ExecuteAsync(id, userId);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("registrations.deleteUser")
        .WithSummary("Unregister a user")
        .WithDescription("Removes the given user's registration from the event.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

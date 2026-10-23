using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record UpdateRegistrationRoleRequest
{
    [Required]
    public string EventRoleId { get; init; } = null!;
    [Required]
    public string UserId { get; init; } = null!;
}

public static class UpdateRegistrationRoleEndpoint
{
    public static void MapUpdateRegistrationRole(this IEndpointRouteBuilder registrations)
    {
        registrations.MapPatch("/", [Authorize][Roles(Perms.HandleEvent)] async (UpdateRegistrationRoleCommand command, string id, UpdateRegistrationRoleRequest req) =>
        {
            var result = await command.ExecuteAsync(id, req);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("registrations.updateRole")
        .WithSummary("Change the role of a registration")
        .WithDescription("Changes the role a user is registered with on the event. The new role must be offered by the event.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

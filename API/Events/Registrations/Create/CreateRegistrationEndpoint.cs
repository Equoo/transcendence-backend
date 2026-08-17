using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record CreateRegistrationRequest
{
    [Required]
    public string EventRoleId { get; init; } = null!;
}

public static class CreateRegistrationEndpoint
{
    public static void MapCreateRegistration(this IEndpointRouteBuilder registrations)
    {
        registrations.MapPost("/", [Authorize] async (CreateRegistrationCommand command, string id, TokenContext context, CreateRegistrationRequest reg) =>
        {
            var result = await command.ExecuteAsync(id, context.User, reg);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.CreatedAtRoute("events.get", new { id });
        })
        .WithName("registrations.create")
        .WithSummary("Register to an event")
        .WithDescription("Registers the current user to the event with the requested role. Fails if the user is already registered, if the event has reached its capacity, or if the event does not offer that role.")
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

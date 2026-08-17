using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record CreateEventRoleRequest
{
    [Required]
    [Length(1, 255)]
    public string Name { get; init; } = null!;
}

public record CreateEventRoleResponse(string Id, string Name)
{
    public static CreateEventRoleResponse FromEntity(EventRole er) => new(er.Id, er.Name);
}

public static class CreateEventRoleEndpoint
{
    public static void MapCreateEventRole(this IEndpointRouteBuilder group)
    {
        group.MapPost("/", [Authorize] async (CreateEventRoleCommand command, CreateEventRoleRequest req) =>
        {
            var result = await command.ExecuteAsync(req);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("eventroles.create")
        .WithSummary("Create an event role")
        .WithDescription("Creates a new role that events can offer. Role names are unique, and the reserved `Any` name is refused.")
        .Produces<CreateEventRoleResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }
}

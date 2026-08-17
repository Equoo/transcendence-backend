using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record CreateEventRequest
{
    [Required]
    [Length(1, 255)]
    public string Name { get; init; } = null!;
    [Required]
    public DateTime Date { get; init; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Size { get; init; }
    [Required]
    [Length(1, 255)]
    public string Location { get; init; } = null!;

    public ICollection<string> Tags { get; init; } = [];
    public ICollection<string> EventRoleIds { get; init; } = [];
    public string Description { get; init; } = string.Empty;
    public ICollection<string> FileKeys { get; init; } = [];
}
public record CreateEventResponse(string Id)
{
    public static CreateEventResponse FromEntity(Event ev) => new(
        ev.Id);
}

public static class CreateEventEndpoint
{
    public static void MapCreateEvent(this IEndpointRouteBuilder events)
    {
        events.MapPost("/", [Authorize] async (CreateEventCommand command, CreateEventRequest req, TokenContext token) =>
        {
            var result = await command.ExecuteAsync(req, token.User);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.CreatedAtRoute("events.get", new { id = result.Value.Id }, result.Value);
        })
        .WithName("events.create")
        .WithSummary("Create an event")
        .WithDescription("Creates an event owned by the current user. The roles listed in `eventRoleIds` are attached to the event, plus the implicit `Any` role. Unknown role ids or file keys are rejected.")
        .Produces<CreateEventResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}

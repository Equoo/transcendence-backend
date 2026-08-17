using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record UpdateEventRequest
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

    public string Description { get; init; } = string.Empty;
    public ICollection<string> Tags { get; init; } = [];
}

public static class UpdateEventEndpoint
{
    public static void MapUpdateEvent(this IEndpointRouteBuilder events)
    {
        events.MapPut("/{id}", [Authorize] async (UpdateEventCommand command, string id, UpdateEventRequest req) =>
        {
            var result = await command.ExecuteAsync(id, req);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("events.update")
        .WithSummary("Update an event")
        .WithDescription("Replaces the editable fields of an event. The organizer, the attached roles and the attached files are not modified. The size cannot be lowered below the number of current participants.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

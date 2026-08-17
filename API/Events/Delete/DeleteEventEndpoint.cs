using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public static class DeleteEventEndpoint
{
    public static void MapDeleteEvent(this IEndpointRouteBuilder events)
    {
        events.MapDelete("/{id}", [Authorize] async (DeleteEventCommand command, string id) =>
        {
            var result = await command.ExecuteAsync(id);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("events.delete")
        .WithSummary("Delete an event")
        .WithDescription("Deletes an event and every registration attached to it.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

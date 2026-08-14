using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Events;

/// <summary>The detail view of an event: everything the API knows about it.</summary>
public record GetEventResponse(
    string Id,
    string Name,
    DateTime Date,
    int Size,
    string Location,
    string Description,
    UserSummary Organizer,
    ICollection<string> Tags,
    ICollection<RegistrationSummary> Registrations,
    ICollection<EventRoleSummary> EventRoles,
    ICollection<FileSummary> Files
)
{
    public static GetEventResponse FromEntity(Event ev) => new(
        ev.Id, ev.Name, ev.Date, ev.Size, ev.Location, ev.Description,
        UserSummary.FromEntity(ev.Organizer), ev.Tags,
        [.. ev.Registrations.Select(RegistrationSummary.FromEntity)],
        [.. ev.EventRoles.Select(EventRoleSummary.FromEntity)],
        [.. ev.Files.Select(FileSummary.FromEntity)]);
}

public static class GetEventEndpoint
{
    public static void MapGetEvent(this IEndpointRouteBuilder events)
    {
        events.MapGet("/{id}", async (GetEventQuery query, string id) =>
        {
            var result = await query.ExecuteAsync(id);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("events.get")
        .WithSummary("Get an event")
        .WithDescription("Returns a single event identified by its id, with its organizer, registrations, available roles and attached files.")
        .Produces<GetEventResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

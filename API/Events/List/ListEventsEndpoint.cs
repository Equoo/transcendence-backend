using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

/// <summary>
/// An event as it appears in a list: enough to render a card and decide whether to open it.
/// The registrations and the attached files are replaced by their counts — fetch the event itself for those.
/// </summary>
public record ListEventsResponse(
    string Id,
    string Name,
    DateTime Date,
    int Size,
    string Location,
    ICollection<string> Tags,
    ICollection<EventRoleSummary> EventRoles,
    int RegisteredCount
)
{
    public static ListEventsResponse FromEntity(Event ev) => new(
        ev.Id, ev.Name, ev.Date, ev.Size, ev.Location, ev.Tags,
        [.. ev.EventRoles.Select(EventRoleSummary.FromEntity)],
        ev.Registrations.Count);
}

public static class ListEventsEndpoint
{
    public static void MapListEvents(this IEndpointRouteBuilder events)
    {
        events.MapGet("/", [Authorize] async (ListEventsQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("events.list")
        .WithSummary("List events")
        .WithDescription("Returns every event with its organizer and available roles, plus how many participants and files it has.")
        .Produces<IEnumerable<ListEventsResponse>>(StatusCodes.Status200OK);
    }
}

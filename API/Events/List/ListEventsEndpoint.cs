using KeepGrouped.API.Users;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public static class ListEventsEndpoint
{
    public static void MapListEvents(this IEndpointRouteBuilder events)
    {
        events.MapGet("/", [Authorize] async (ListEventsQuery query, TokenContext token) =>
        {
            var result = await query.ExecuteAsync(token.User);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("events.list")
        .WithSummary("List events")
        .WithDescription("Returns every event with its organizer and available roles, plus how many participants and files it has.")
        .Produces<IEnumerable<EventSummary>>(StatusCodes.Status200OK);
    }
}

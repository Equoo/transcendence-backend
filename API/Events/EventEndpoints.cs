namespace KeepGrouped.API.Events;

public static class EventEndpoints
{
    public static void MapEvents(this IEndpointRouteBuilder app)
    {
        var events = app.MapGroup("/events").WithTags("Events");

        events.MapCreateEvent();
        events.MapListEvents();
        events.MapGetEvent();
        events.MapUpdateEvent();
        events.MapDeleteEvent();
    }
}

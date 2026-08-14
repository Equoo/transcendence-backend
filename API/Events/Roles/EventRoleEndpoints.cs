namespace KeepGrouped.API.Events;

public static class EventRoleEndpoints
{
    public static void MapEventRoles(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/events/roles/").WithTags("Event roles");

        group.MapListEventRoles();
        group.MapCreateEventRole();
    }
}

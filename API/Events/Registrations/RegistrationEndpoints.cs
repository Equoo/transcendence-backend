namespace KeepGrouped.API.Events;

public static class RegistrationEndpoints
{
    public static void MapRegistrations(this IEndpointRouteBuilder app)
    {
        var registrations = app.MapGroup("/events/{id}/registration").WithTags("Registrations");

        registrations.MapCreateRegistration();
        registrations.MapListRegistrations();
        registrations.MapDeleteRegistration();
    }
}

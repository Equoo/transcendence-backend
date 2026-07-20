using KeepGrouped.API.Users;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public class Registration
{
    public User User { get; set; } = null!;

    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public EventRole? Role { get; set; }
}

public record RegistrationResponse(UserResponse User, DateTime RegisteredAt, string? Role)
{
    public static RegistrationResponse FromEntity(Registration reg) => new(
        UserResponse.FromEntity(reg.User),
        reg.RegisteredAt,
        reg.Role?.Name);
}

public record RegistrationCreate(string EventRoleId);

public static class RegistrationEndpoints
{
    public static void MapRegistrations(this IEndpointRouteBuilder app)
    {
        var registrations = app.MapGroup("/events/{id}/registration").WithTags("Registrations");

        registrations.MapPost("/", [Authorize] async (KeepGroupedDb db, string id, TokenContext context, RegistrationCreate reg) =>
        {

            Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
            // Fetch user with authentication
            EventRole? eventRole = await db.EventRoles.SingleOrDefaultAsync(er => er.Id == reg.EventRoleId);

            if ((ev is null) || (context.User is null) || (eventRole is null))
            {
                return Results.NotFound();
            }
            if (ev.Users.Contains(context.User))
            {
                return EventProblems.AlreadyRegistered(context.User.UserName!, ev.Name);
            }
            if (ev.Users.Count >= ev.Size)
            {
                return EventProblems.EventFull();
            }
           
            ev.Registrations.Add(new Registration()
            {
                User = context.User,
                Role = eventRole
            });

            await db.SaveChangesAsync();
            return Results.Created();
        })
        .WithName("registrations.create")
        .WithSummary("Register to an event")
        .WithDescription("Registers the current user to the event with the requested role. Fails if the user is already registered or if the event has reached its capacity.")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        registrations.MapGet("/", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);

            if (ev is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(ev.Registrations.Select(RegistrationResponse.FromEntity));
        })
        .WithName("registrations.list")
        .WithSummary("List the registrations of an event")
        .WithDescription("Returns every user registered to the event, with their role and registration date.")
        .Produces<IEnumerable<RegistrationResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        registrations.MapDelete("/", async (KeepGroupedDb db, string id, TokenContext token) =>
        {
            Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
            // Fetch user with authentication

            if ((ev is null) || (token.User is null))
            {
                return Results.NotFound();
            }
            if (!ev.Users.Contains(token.User))
            {
                return EventProblems.NotRegistered(token.User.UserName!, ev.Name);
            }
            ev.Users.Remove(token.User);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("registrations.delete")
        .WithSummary("Cancel a registration")
        .WithDescription("Removes the current user's registration from the event.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
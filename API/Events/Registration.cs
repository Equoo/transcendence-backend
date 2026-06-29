using KeepGrouped.API.Users;
using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public class Registration
{
    public ApplicationUser User { get; set; } = null!;

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

public static class RegistrationEndpoints
{
    public static void Map(WebApplication app)
    {
        var registrations = app.MapGroup("/events/{id}/registration");

        registrations.MapPost("/", async (KeepGroupedDb db, string id) =>
        {
            Thread.Sleep(500);
            Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
            // Fetch user with authentication
            ApplicationUser? user = await db.Users.SingleOrDefaultAsync(u => u.UserName == "asventi");

            if ((ev is null) || (user is null))
            {
                return Results.NotFound();
            }
            if (ev.Users.Contains(user))
            {
                return EventProblems.AlreadyRegistered(user.UserName!, ev.Name);
            }
            if (ev.Users.Count >= ev.Size)
            {
                return EventProblems.EventFull();
            }
            ev.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created();
        });

        registrations.MapGet("/", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);

            if (ev is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(ev.Registrations.Select(RegistrationResponse.FromEntity));
        });

        registrations.MapDelete("/", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
            // Fetch user with authentication
            ApplicationUser? user = await db.Users.Where(u => u.UserName == "asventi").FirstAsync();

            if ((ev is null) || (user is null))
            {
                return Results.NotFound();
            }
            if (!ev.Users.Contains(user))
            {
                return EventProblems.NotRegistered(user.UserName!, ev.Name);
            }
            ev.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
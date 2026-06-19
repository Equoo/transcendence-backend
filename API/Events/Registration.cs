using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public class Registration
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string EventId { get; set; } = string.Empty;
    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public RegistrationRole Role { get; set; }

}

public enum RegistrationRole
{
    Participant,
    Organizer,
}

public static class RegistrationEndpoints
{
    public static void Map(WebApplication app)
    {
        var events = app.MapGroup("/events/{id}/registration");

        events.MapPost("/", async (KeepGroupedDb db, string id, [FromForm] string role) =>
        {
            Event? ev = await db.Events.FindAsync(id);
            ApplicationUser? user = await db.Users.Where(u => u.UserName == "asventi").FirstAsync();

            if ((ev is null) || (user is null))
            {
                return Results.NotFound();
            }
            ev.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created();
        });
    }
}
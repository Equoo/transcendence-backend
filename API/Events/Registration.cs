using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public class Registration
{
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public string EventId { get; set; } = null!;
    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public string Role { get; set; } = string.Empty;

}

public static class RegistrationEndpoints
{
    public static void Map(WebApplication app)
    {
        var events = app.MapGroup("/events/{id}/registration");

        events.MapPost("/", async (KeepGroupedDb db, string id, [FromForm] string? role) =>
        {
            Event? ev = await db.Events.FindAsync(id);
            // Fetch user with authentication
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
using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;


namespace KeepGrouped.API.Events;

public class Event
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Size { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }

    public ICollection<ApplicationUser> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}

public record CreateEventRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public DateTime Date { get; init; }

    [Range(1, int.MaxValue)]
    public int Size { get; init; }

    [Required]
    public string Location { get; init; } = string.Empty;

    public string? Description { get; init; }
    public ICollection<string>? Tags { get; init; }
}

public record UpdateEventRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public DateTime Date { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Size { get; init; }

    [Required]
    public string Location { get; init; } = string.Empty;

    public string? Description { get; init; }
    public ICollection<string>? Tags { get; init; }
}

public record EventResponse(
    string Id,
    string Name,
    DateTime Date,
    int Size,
    string Location,
    string? Description,
    ICollection<string>? Tags)
{
    public static EventResponse FromEntity(Event ev) => new(
        ev.Id, ev.Name, ev.Date, ev.Size, ev.Location, ev.Description, ev.Tags);
}

public static class EventEndpoints
{
    public static void Map(WebApplication app)
    {
        var events = app.MapGroup("/events");

        events.MapPost("/", async (KeepGroupedDb db, CreateEventRequest req) =>
        {
            var ev = new Event
            {
                Name = req.Name,
                Date = req.Date.ToUniversalTime(),
                Size = req.Size,
                Location = req.Location,
                Description = req.Description,
                Tags = req.Tags,
            };

            db.Events.Add(ev);
            await db.SaveChangesAsync();

            var response = EventResponse.FromEntity(ev);
            return Results.Created($"/events/{ev.Id}", response);
        });

        events.MapGet("/", async (KeepGroupedDb db) =>
            await db.Events
                .Select(ev => EventResponse.FromEntity(ev))
                .ToListAsync());

        events.MapGet("/{id}", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.FindAsync(id);
            return ev is null ? Results.NotFound() : Results.Ok(EventResponse.FromEntity(ev));
        });

        events.MapPut("/{id}", async (KeepGroupedDb db, string id, UpdateEventRequest req) =>
        {
            Event? ev = await db.Events.FindAsync(id);
            if (ev is null)
            {
                return Results.NotFound();
            }

            ev.Name = req.Name;
            ev.Date = req.Date.ToUniversalTime();
            ev.Size = req.Size;
            ev.Location = req.Location;
            ev.Description = req.Description;
            ev.Tags = req.Tags;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        events.MapDelete("/{id}", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.FindAsync(id);
            if (ev is null)
            {
                return Results.NotFound();
            }

            db.Events.Remove(ev);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}

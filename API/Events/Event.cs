using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace KeepGrouped.API.Events;

public class Event
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }
    public int Size { get; set; }
    public string Location { get; set; } = null!;
    public ICollection<string> Tags { get; set; } = [];
    public string Description { get; set; } = string.Empty;

    public ICollection<ApplicationUser> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}

public record CreateEventRequest
{
    [Required]
    public string Name { get; init; } = null!;

    [Required]
    public DateTime Date { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Size { get; init; }

    [Required]
    public string Location { get; init; } = null!;

    public ICollection<string> Tags { get; init; } = [];

    public string Description { get; init; } = string.Empty;
}

public record EventResponse(
    string Id,
    string Name,
    DateTime Date,
    int Size,
    string Location,
    string Description,
    ICollection<string> Tags,
    ICollection<RegistrationResponse> Registrations)
{
    public static EventResponse FromEntity(Event ev) => new(
        ev.Id, ev.Name, ev.Date, ev.Size, ev.Location, ev.Description, ev.Tags,
        [.. ev.Registrations.Select(RegistrationResponse.FromEntity)]);
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
        }).DisableAntiforgery();

        events.MapGet("/", async (KeepGroupedDb db) =>
        {
            var evs = await db.Events.Include(ev => ev.Registrations).ThenInclude(r => r.User).ToListAsync();
            return evs.Select(EventResponse.FromEntity);
        });

        events.MapGet("/{id}", async (KeepGroupedDb db, string id) =>
        {
            var ev = await db.Events.Include(e => e.Registrations).ThenInclude(r => r.User).SingleOrDefaultAsync(e => e.Id == id);
            return ev is null ? Results.NotFound() : Results.Ok(EventResponse.FromEntity(ev));
        });

        events.MapPut("/{id}", async (KeepGroupedDb db, string id, CreateEventRequest req) =>
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
        }).DisableAntiforgery();

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

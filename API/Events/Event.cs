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

    public User Organizer { get; set; } = null!;
    public ICollection<User> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public ICollection<EventRole> EventRoles { get; init; } = [];
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
    public ICollection<string> EventRolesId { get; init; } = [];
    public string Description { get; init; } = string.Empty;
}

public record EventResponse(
    string Id,
    string Name,
    DateTime Date,
    int Size,
    string Location,
    string Description,
    UserResponse Organizer,
    ICollection<string> Tags,
    ICollection<RegistrationResponse> Registrations,
    ICollection<EventRoleResponse> EventRoles)
{
    public static EventResponse FromEntity(Event ev) => new(
        ev.Id, ev.Name, ev.Date, ev.Size, ev.Location, ev.Description,
        UserResponse.FromEntity(ev.Organizer), ev.Tags,
        [.. ev.Registrations.Select(RegistrationResponse.FromEntity)],
        [.. ev.EventRoles.Select(EventRoleResponse.FromEntity)]);
}

public static class EventEndpoints
{
    public static void MapEvents(this IEndpointRouteBuilder app)
    {
        var events = app.MapGroup("/events");

        events.MapPost("/", async (KeepGroupedDb db, CreateEventRequest req) =>
        {
            // Replace with authentication devan pitie j'en ai marre de faire sans
            User? user = await db.Users.FirstOrDefaultAsync(u => u.UserName == "asventi");

            if (user == null)
            {
                return Results.Unauthorized();
            }
            var ev = new Event
            {
                Name = req.Name,
                Date = req.Date.ToUniversalTime(),
                Size = req.Size,
                Location = req.Location,
                Description = req.Description,
                Organizer = user,
                EventRoles = await db.EventRoles.Where(er => req.EventRolesId.Contains(er.Id)).ToListAsync(),
                Tags = req.Tags,
            };

            EventRole anyRole = await db.EventRoles.SingleAsync(er => er.Name == "Any");

            ev.EventRoles.Add(anyRole);
            db.Events.Add(ev);
            await db.SaveChangesAsync();

            var response = EventResponse.FromEntity(ev);
            return Results.Created($"/events/{ev.Id}", response);
        });

        events.MapGet("/", async (KeepGroupedDb db) =>
        {
            var evs = await db.Events.ToListAsync();
            return Results.Ok(evs.Select(EventResponse.FromEntity));
        });

        events.MapGet("/{id}", async (KeepGroupedDb db, string id) =>
        {
            var ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
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

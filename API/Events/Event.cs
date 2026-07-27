using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Users;
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

public record EventCreate
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
    public ICollection<string> EventRoleIds { get; init; } = [];
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
        var events = app.MapGroup("/events").WithTags("Events");

        events.MapPost("/", async (KeepGroupedDb db, EventCreate req) =>
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
                EventRoles = await db.EventRoles.Where(er => req.EventRoleIds.Contains(er.Id)).ToListAsync(),
                Tags = req.Tags,
            };

            EventRole anyRole = await db.EventRoles.SingleAsync(er => er.Name == "Any");

            ev.EventRoles.Add(anyRole);
            db.Events.Add(ev);
            await db.SaveChangesAsync();

            var response = EventResponse.FromEntity(ev);
            return Results.CreatedAtRoute("events.get", new { id = ev.Id }, response);
        })
        .WithName("events.create")
        .WithSummary("Create an event")
        .WithDescription("Creates an event owned by the current user. The roles listed in `eventRoleIds` are attached to the event, plus the implicit `Any` role.")
        .Produces<EventResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        events.MapGet("/", async (KeepGroupedDb db) =>
        {
            var evs = await db.Events.ToListAsync();
            return Results.Ok(evs.Select(EventResponse.FromEntity));
        })
        .WithName("events.list")
        .WithSummary("List events")
        .WithDescription("Returns every event with its organizer, registrations and available roles.")
        .Produces<IEnumerable<EventResponse>>(StatusCodes.Status200OK);

        events.MapGet("/{id}", async (KeepGroupedDb db, string id) =>
        {
            var ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
            return ev is null ? Results.NotFound() : Results.Ok(EventResponse.FromEntity(ev));
        })
        .WithName("events.get")
        .WithSummary("Get an event")
        .WithDescription("Returns a single event identified by its id, with its organizer, registrations and available roles.")
        .Produces<EventResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        events.MapPut("/{id}", async (KeepGroupedDb db, string id, EventCreate req) =>
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
        })
        .WithName("events.update")
        .WithSummary("Update an event")
        .WithDescription("Replaces the editable fields of an event. The organizer and the attached roles are not modified.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

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
        })
        .WithName("events.delete")
        .WithSummary("Delete an event")
        .WithDescription("Deletes an event and every registration attached to it.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

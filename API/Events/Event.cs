using System.Data;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace KeepGrouped.API.Events;

class EvenementConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
    }
}

[EntityTypeConfiguration(typeof(EvenementConfiguration))]
public class Event
{
    public Event()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }
    required public string Name { get; set; }
    required public DateTime Date { get; set; }
    required public int Size { get; set; }
    public string? Description { get; set; }

    public ICollection<ApplicationUser> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}

public record EventPost(string Name, DateTime Date, int Size, string? Description);

public static class EventEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/events", async (KeepGroupedDb db, EventPost ev_req) =>
        {
            var ev = new Event()
            {
                Name = ev_req.Name,
                Size = ev_req.Size,
                Date = ev_req.Date,
                Description = ev_req.Description
            };
            db.Add(ev);
            await db.SaveChangesAsync();
            return Results.Created($"/events/{ev.Id}", ev);
        }).DisableAntiforgery();

        app.MapGet("/events", async (KeepGroupedDb db) =>
        {
            return await db.Events.ToListAsync();
        });

        app.MapGet("/events/{id}", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.FindAsync(id);
            return ev is null ? Results.NotFound() : Results.Ok(ev);
        });

        app.MapPut("/events/{id}", async (KeepGroupedDb db, string id, EventPost ev_req) =>
        {
            Event? ev = await db.Events.FindAsync(id);

            if (ev is null)
            {
                return Results.NotFound();
            }
            db.Entry(ev).CurrentValues.SetValues(ev_req);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).DisableAntiforgery();

        app.MapDelete("/events/{id}", async (KeepGroupedDb db, string id) =>
        {
            Event? ev = await db.Events.FindAsync(id);

            if (ev is null)
            {
                return Results.NotFound();
            }
            db.Events.Remove(ev);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).DisableAntiforgery();
    }
}
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
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
    [SetsRequiredMembers]
    public Event()
    {
        Id = Guid.NewGuid().ToString();
        Name = "Default Event";
        Size = 20;
        Date = DateTime.UtcNow;
        Location = "Here";
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    required public string Id { get; set; }
    required public string Name { get; set; }
    required public DateTime Date { get; set; }

    [Range(1, int.MaxValue)]
    required public int Size { get; set; }
    required public string Location { get; set; }
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public ICollection<ApplicationUser> Users { get; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public ICollection<Registration> Registrations { get; } = [];
}

public static class EventEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/events", async (KeepGroupedDb db, Event ev) =>
        {
            ev.Date = ev.Date.ToUniversalTime();
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

        app.MapPut("/events/{id}", async (KeepGroupedDb db, string id, Event ev_req) =>
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
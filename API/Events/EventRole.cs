using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;
namespace KeepGrouped.API.Events;

public class EventRole
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public ICollection<Event> Events { get; } = [];
}

public record CreateEventRole
{
    [Required]
    [Length(1, 255)]
    public string Name { get; set; } = null!;
}

public record EventRoleResponse(string Id, string Name)
{
    public static EventRoleResponse FromEntity(EventRole er) => new(er.Id, er.Name);
}

public static class EventRoleEndpoints
{
    public static void MapEventRoles(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/events/roles/");

        group.MapGet("/", async (KeepGroupedDb db) =>
        {
            var ers = await db.EventRoles.Where(er => er.Name != "Any").ToListAsync();
            return Results.Ok(ers.Select(EventRoleResponse.FromEntity));
        });

        group.MapPost("/", async (KeepGroupedDb db, CreateEventRole req) =>
        {
            if (db.EventRoles.Any(er => er.Name == req.Name))
            {
                return EventProblems.EventRoleAlreadyExists(req.Name);
            }

            EventRole er = new() { Name = req.Name };

            db.EventRoles.Add(er);
            await db.SaveChangesAsync();
            return Results.Ok(EventRoleResponse.FromEntity(er));
        });
    }
}

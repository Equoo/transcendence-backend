using System.ComponentModel.DataAnnotations;
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
        var group = app.MapGroup("/events/roles/").WithTags("Event roles");

        group.MapGet("/", async (KeepGroupedDb db) =>
        {
            var ers = await db.EventRoles.Where(er => er.Name != "Any").ToListAsync();
            return Results.Ok(ers.Select(EventRoleResponse.FromEntity));
        })
        .WithName("eventroles.list")
        .WithSummary("List the event roles")
        .WithDescription("Returns every role that can be attached to an event. The implicit `Any` role is excluded.")
        .Produces<IEnumerable<EventRoleResponse>>(StatusCodes.Status200OK);

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
        })
        .WithName("eventroles.create")
        .WithSummary("Create an event role")
        .WithDescription("Creates a new role that events can offer. Role names are unique.")
        .Produces<EventRoleResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

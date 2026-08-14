using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class UpdateEventCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string id, UpdateEventRequest req)
    {
        Event? ev = await db.Events
            .Include(e => e.Registrations)
            .SingleOrDefaultAsync(e => e.Id == id);
        if (ev is null)
        {
            return EventProblems.NotFound(id);
        }

        if (req.Size < ev.Registrations.Count)
        {
            return EventProblems.SizeBelowRegistrations(req.Size, ev.Registrations.Count);
        }

        ev.Name = req.Name;
        ev.Date = req.Date.ToUniversalTime();
        ev.Size = req.Size;
        ev.Location = req.Location;
        ev.Description = req.Description;
        ev.Tags = req.Tags;

        await db.SaveChangesAsync();
        return Result.Success;
    }
}

using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;
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

        List<EventRole> roles = await db.EventRoles.Where(er => req.EventRoleIds.Contains(er.Id)).ToListAsync();
        if (roles.Count != req.EventRoleIds.Distinct().Count())
        {
            return EventProblems.UnknownEventRoles(req.EventRoleIds.Except(roles.Select(er => er.Id)));
        }

        List<StorageFile> files = await db.Files.Where(fi => req.FileKeys.Contains(fi.Key)).ToListAsync();
        if (files.Count != req.FileKeys.Distinct().Count())
        {
            return EventProblems.UnknownFiles(req.FileKeys.Except(files.Select(fi => fi.Key)));
        }

        ev.Name = req.Name;
        ev.Date = req.Date.ToUniversalTime();
        ev.Size = req.Size;
        ev.Location = req.Location;
        ev.Description = req.Description;
        ev.Tags = req.Tags;
        ev.Files = files;
        ev.EventRoles = roles;

        await db.SaveChangesAsync();
        return Result.OK;
    }
}

using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Events;

public sealed class ListEventsQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<EventSummary>>> ExecuteAsync(User? user)
    {
        var evs = await db.Events
            .AsNoTracking()
            .Include(e => e.EventRoles)
            .Include(e => e.Registrations)
            .ToListAsync();

        return evs.Select(ev => EventSummary.FromEntity(ev, user is not null && ev.Registrations.Any(r => r.User.Id == user.Id))).ToList();
    }
}

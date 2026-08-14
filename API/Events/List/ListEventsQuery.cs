using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class ListEventsQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListEventsResponse>>> ExecuteAsync()
    {
        var evs = await db.Events
            .AsNoTracking()
            .Include(e => e.EventRoles)
            .Include(e => e.Registrations)
            .ToListAsync();

        return evs.Select(ListEventsResponse.FromEntity).ToList();
    }
}

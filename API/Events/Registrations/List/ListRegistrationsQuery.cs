using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class ListRegistrationsQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListRegistrationsResponse>>> ExecuteAsync(string eventId)
    {
        Event? ev = await db.Events
            .AsNoTracking()
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .Include(e => e.Registrations).ThenInclude(r => r.Role)
            .SingleOrDefaultAsync(e => e.Id == eventId);
        if (ev is null)
        {
            return EventProblems.NotFound(eventId);
        }

        return ev.Registrations.Select(ListRegistrationsResponse.FromEntity).ToList();
    }
}

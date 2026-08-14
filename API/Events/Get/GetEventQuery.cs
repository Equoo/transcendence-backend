using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class GetEventQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<GetEventResponse>> ExecuteAsync(string id)
    {
        var ev = await db.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .Include(e => e.EventRoles)
            .Include(e => e.Files)
            .SingleOrDefaultAsync(e => e.Id == id);

        return ev is null ? EventProblems.NotFound(id) : GetEventResponse.FromEntity(ev);
    }
}

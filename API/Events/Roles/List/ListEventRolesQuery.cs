using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class ListEventRolesQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<ListEventRolesResponse>>> ExecuteAsync()
    {
        var ers = await db.EventRoles.Where(er => er.Name != EventRole.Implicit).ToListAsync();

        return ers.Select(ListEventRolesResponse.FromEntity).ToList();
    }
}

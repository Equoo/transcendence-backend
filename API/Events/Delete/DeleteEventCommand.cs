using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class DeleteEventCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string id)
    {
        Event? ev = await db.Events.SingleOrDefaultAsync(e => e.Id == id);
        if (ev is null)
        {
            return EventProblems.NotFound(id);
        }

        db.Events.Remove(ev);
        await db.SaveChangesAsync();
        return Result.OK;
    }
}

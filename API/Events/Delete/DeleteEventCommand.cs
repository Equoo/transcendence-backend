using KeepGrouped.API.AiBackend.Delete;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Rag;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class DeleteEventCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub, RagQueue rag) : IHandler
{
    public async Task<Result> ExecuteAsync(string id)
    {
        Event? ev = await db.Events.Include(e => e.Channel).SingleOrDefaultAsync(e => e.Id == id);
        if (ev is null)
        {
            return EventProblems.NotFound(id);
        }

        db.Events.Remove(ev);
        db.Channels.Remove(ev.Channel);
        await db.SaveChangesAsync();

        await hub.Clients.All.SendAsync("RemoveChannel", ev.ChannelId);

        rag.Delete(ev);
        return Result.OK;
    }
}

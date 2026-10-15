using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class DeleteRelationshipCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
    public async Task<Result> ExecuteAsync(User me, string userId)
    {
        Relationship? mine = await db.Relationships.SingleOrDefaultAsync(r => r.MeId == me.Id && r.UserId == userId);
        if (mine is null)
        {
            return RelationshipProblems.NotFound(userId);
        }

        db.Relationships.Remove(mine);

        Relationship? theirs = await db.Relationships.SingleOrDefaultAsync(r => r.MeId == userId && r.UserId == me.Id);
        bool removeTheirs = theirs is not null && theirs.Type != RelationshipState.Blocked;
        if (removeTheirs)
        {
            db.Relationships.Remove(theirs!);
        }

        await db.SaveChangesAsync();

        await hub.Clients.User(me.Id).SendAsync("RemoveRelationship", userId);
        if (removeTheirs)
        {
            await hub.Clients.User(userId).SendAsync("RemoveRelationship", me.Id);
        }

        return Result.OK;
    }
}

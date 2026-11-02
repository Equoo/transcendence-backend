using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class DeleteRelationshipCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
    public async Task<Result> ExecuteAsync(User me, string userId)
    {
        await using var tx = await db.LockPairAsync(me.Id, userId);
        Relationship? mine = await db.Relationships.SingleOrDefaultAsync(r => r.MeId == me.Id && r.UserId == userId);
        if (mine is null)
        {
            return RelationshipProblems.NotFound(userId);
        }

        Relationship? theirs = await db.Relationships.SingleOrDefaultAsync(r => r.MeId == userId && r.UserId == me.Id);

        if (mine.Type == RelationshipState.Blocked && theirs?.Type == RelationshipState.PendingRequest)
        {
            await db.Entry(mine).Reference(r => r.User).LoadAsync();
            mine.Type = RelationshipState.PendingResponse;
            await db.SaveChangesAsync();
            await tx.CommitAsync();

            await hub.Clients.User(me.Id).SendAsync("UpsertRelationship", RelationshipResponse.FromEntity(mine));
            return Result.OK;
        }

        db.Relationships.Remove(mine);
        bool removeTheirs = theirs is not null && theirs.Type != RelationshipState.Blocked;
        if (removeTheirs)
        {
            db.Relationships.Remove(theirs!);
        }

        await db.SaveChangesAsync();

        await tx.CommitAsync();

        await hub.Clients.User(me.Id).SendAsync("RemoveRelationship", userId);
        if (removeTheirs)
        {
            await hub.Clients.User(userId).SendAsync("RemoveRelationship", me.Id);
        }

        return Result.OK;
    }
}

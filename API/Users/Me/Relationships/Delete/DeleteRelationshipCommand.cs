using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class DeleteRelationshipCommand(KeepGroupedDb db) : IHandler
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
        if (theirs is not null && theirs.Type != RelationshipState.Blocked)
        {
            db.Relationships.Remove(theirs);
        }

        await db.SaveChangesAsync();
        return Result.OK;
    }
}

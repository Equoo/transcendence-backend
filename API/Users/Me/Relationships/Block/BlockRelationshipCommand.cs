using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class BlockRelationshipCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result<RelationshipResponse>> ExecuteAsync(User me, string userId)
    {
        if (me.Id == userId)
        {
            return RelationshipProblems.SelfRelationship();
        }

        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return UserProblems.NotFound(userId);
        }

        Relationship? mine = await db.Relationships.SingleOrDefaultAsync(r => r.MeId == me.Id && r.UserId == userId);
        Relationship? theirs = await db.Relationships.SingleOrDefaultAsync(r => r.MeId == userId && r.UserId == me.Id);

        if (mine is null)
        {
            mine = new() { MeId = me.Id, Me = me, UserId = user.Id, User = user };
            db.Relationships.Add(mine);
        }
        mine.Type = RelationshipState.Blocked;

        // Their own block stays: blocks are one-sided.
        if (theirs is not null && theirs.Type != RelationshipState.Blocked)
        {
            db.Relationships.Remove(theirs);
        }

        await db.SaveChangesAsync();
        return RelationshipResponse.FromEntity(mine);
    }
}

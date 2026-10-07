using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class CreateRelationshipCommand(KeepGroupedDb db) : IHandler
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

        if (theirs?.Type == RelationshipState.Blocked)
        {
            return RelationshipProblems.Blocked(user.UserName);
        }

        // Requesting someone who already requested us accepts their request.
        if (mine?.Type == RelationshipState.PendingResponse && theirs is not null)
        {
            mine.Type = RelationshipState.Friend;
            theirs.Type = RelationshipState.Friend;
        }
        else if (mine is not null)
        {
            return RelationshipProblems.AlreadyExists(user.UserName);
        }
        else
        {
            mine = new() { MeId = me.Id, Me = me, UserId = user.Id, User = user, Type = RelationshipState.PendingRequest };
            db.Relationships.Add(mine);
            db.Relationships.Add(new() { MeId = user.Id, Me = user, UserId = me.Id, User = me, Type = RelationshipState.PendingResponse });
        }

        await db.SaveChangesAsync();
        return RelationshipResponse.FromEntity(mine);
    }
}

using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class UpdateRelationshipCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result<RelationshipResponse>> ExecuteAsync(User me, string userId, UpdateRelationshipRequest req)
    {
        Relationship? relationship = await db.Relationships
            .Include(r => r.User)
            .SingleOrDefaultAsync(r => r.MeId == me.Id && r.UserId == userId);
        if (relationship is null)
        {
            return RelationshipProblems.NotFound(userId);
        }

        relationship.Nickname = req.Nickname;

        await db.SaveChangesAsync();
        return RelationshipResponse.FromEntity(relationship);
    }
}

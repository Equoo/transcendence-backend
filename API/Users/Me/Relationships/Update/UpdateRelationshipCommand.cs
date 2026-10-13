using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class UpdateRelationshipCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
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

        // Nicknames are private: only the current user's other sessions hear about it.
        var response = RelationshipResponse.FromEntity(relationship);
        await hub.Clients.User(me.Id).SendAsync("UpsertRelationship", response);

        return response;
    }
}

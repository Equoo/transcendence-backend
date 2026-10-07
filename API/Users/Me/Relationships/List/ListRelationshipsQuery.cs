using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public sealed class ListRelationshipsQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<RelationshipResponse>>> ExecuteAsync(string meId)
    {
        List<Relationship> relationships = await db.Relationships
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.MeId == meId)
            .ToListAsync();

        return relationships.Select(RelationshipResponse.FromEntity).ToList();
    }
}

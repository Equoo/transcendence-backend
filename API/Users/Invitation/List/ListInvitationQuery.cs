using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Invitation;

public sealed class ListInvitationQuery(KeepGroupedDb db) : IHandler
{
    private readonly KeepGroupedDb _db = db;

    public async Task<Result<List<Invitation>>> ExecuteAsync()
    {
        var invitations = await _db.Invitations.AsNoTracking().ToListAsync();

        return invitations;
    }
}
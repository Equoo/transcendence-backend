using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Invitation;

public sealed class DeleteInvitationCommand(KeepGroupedDb db) : IHandler
{
    private readonly KeepGroupedDb _db = db;

    public async Task<Result> ExecuteAsync(string id)
    {
        Invitation? invitation = await _db.Invitations.SingleOrDefaultAsync(i => i.Id == id);
        if (invitation is null)
        {
            return InvitationProblems.NotFound(id);
        }

        _db.Invitations.Remove(invitation);
        await _db.SaveChangesAsync();
        return Result.OK;
    }
}

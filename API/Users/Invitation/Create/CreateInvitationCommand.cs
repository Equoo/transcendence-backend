using KeepGrouped.API;
using KeepGrouped.API.Users.Invitation;

public sealed class CreateInvitationCommand(KeepGroupedDb db) : IHandler
{
    private readonly KeepGroupedDb _db = db;

    public async Task<Result> ExecuteAsync(CreateInvitationRequest req)
    {
        Invitation invitation = new()
        {
            ExpiresAt = req.ExpiresAt,
            Usages = req.Usages
        };

        _db.Invitations.Add(invitation);
        await _db.SaveChangesAsync();
        return Result.Success;
    }
}
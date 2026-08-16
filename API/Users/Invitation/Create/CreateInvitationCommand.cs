namespace KeepGrouped.API.Users.Invitation;

public sealed class CreateInvitationCommand(KeepGroupedDb db) : IHandler
{
    private readonly KeepGroupedDb _db = db;

    public async Task<Result<string>> ExecuteAsync(CreateInvitationRequest req)
    {
        Invitation invitation = new()
        {
            ExpiresAt = req.ExpiresAt.ToUniversalTime(),
            Usages = req.Usages
        };

        _db.Invitations.Add(invitation);
        await _db.SaveChangesAsync();
        return invitation.Id;
    }
}
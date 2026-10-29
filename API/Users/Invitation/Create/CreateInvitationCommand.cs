namespace KeepGrouped.API.Users.Invitations;

public sealed class CreateInvitationCommand(KeepGroupedDb db) : IHandler
{
    private readonly KeepGroupedDb _db = db;

    public async Task<Result<string>> ExecuteAsync(CreateInvitationRequest req)
    {
        Invitation invitation = new()
        {

            ExpireAt = req.Usages!.Value == -1 ? DateTime.MaxValue : req.ExpireAt!.Value.ToUniversalTime(),
            Usages = req.Usages.Value
        };

        _db.Invitations.Add(invitation);
        await _db.SaveChangesAsync();
        return invitation.Id;
    }
}
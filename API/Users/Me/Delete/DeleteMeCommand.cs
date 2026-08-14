using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class DeleteMeCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result<DeleteMeResponse>> ExecuteAsync(User user)
    {
        var response = DeleteMeResponse.FromEntity(user);

        // Refresh tokens have no navigation to their user, so nothing cascades: revoke them here or
        // they outlive the account they authenticate.
        await db.RefreshTokens.Where(t => t.UserId == user.Id).ExecuteDeleteAsync();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return response;
    }
}

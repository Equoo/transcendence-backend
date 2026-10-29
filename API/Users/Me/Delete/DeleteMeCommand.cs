using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class DeleteMeCommand(KeepGroupedDb db, IOptions<AuthenticationOptions> option, DeleteBatchFileCommand deleteFiles) : IHandler
{
    public async Task<Result<DeleteMeResponse>> ExecuteAsync(User user)
    {

        if (option.Value.DefaultAdminLogin == user.UserName)
        {
            return UserProblems.UserCannotBeDelete();
        }
        var response = DeleteMeResponse.FromEntity(user);

        // Refresh tokens have no navigation to their user, so nothing cascades: revoke them here or
        // they outlive the account they authenticate.
        await db.RefreshTokens.Where(t => t.UserId == user.Id).ExecuteDeleteAsync();

        var userFiles = await db.Files.Where(fi => fi.Creator.Id == user.Id).ToListAsync();
        await deleteFiles.ExecuteAsync(userFiles.Select(fi => fi.Key));
        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return response;
    }
}

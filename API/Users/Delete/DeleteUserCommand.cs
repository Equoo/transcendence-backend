using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class DeleteUserCommand(KeepGroupedDb db, IOptions<AuthenticationOptions> option, DeleteFileCommand deleteFile) : IHandler
{
    public async Task<Result> ExecuteAsync(string id)
    {
        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return UserProblems.NotFound(id);
        }

        if (option.Value.DefaultAdminLogin == user.UserName)
        {
            return UserProblems.UserCannotBeDelete();
        }

        RefreshToken? refresh = await db.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == id);

        if (refresh is not null)
        {
            db.RefreshTokens.Remove(refresh);
        }
        if (user.Avatar is not null)
        {
            await deleteFile.ExecuteAsync(user.Avatar.Key);
        }
        db.Users.Remove(user);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
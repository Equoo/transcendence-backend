
using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class DeleteUserCommand(KeepGroupedDb db, IOptions<AuthenticationOptions> option, DeleteBatchFileCommand deleteFiles, TokenContext tk) : IHandler
{
    public async Task<Result> ExecuteAsync(string id)
    {
        User? db_user = await db.Users.SingleOrDefaultAsync(u => u.Id == id);

        if (db_user is null)
        {
            return UserProblems.NotFound(id);
        }

        if (tk.User.Role.Ranking > db_user.Role.Ranking)
        {
            return UserProblems.NotAuthorized();
        }

        if (option.Value.DefaultAdminLogin == db_user.UserName)
        {
            return UserProblems.UserCannotBeDelete();
        }

        RefreshToken? refresh = await db.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == id);

        if (refresh is not null)
        {
            db.RefreshTokens.Remove(refresh);
        }
        var userFiles = await db.Files.Where(fi => fi.Creator.Id == db_user.Id).ToListAsync();
        await deleteFiles.ExecuteAsync(userFiles.Select(fi => fi.Key));

        var chans = await db.Channels.Where(chan => chan.Members.Any(member => member.UserId == db_user.Id)
            && chan.Type == Channels.ChannelType.DirectMessage).ToListAsync();

        db.RemoveRange(chans);

        db.Users.Remove(db_user);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
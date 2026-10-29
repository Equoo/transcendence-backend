using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class DeleteUserCommand(KeepGroupedDb db, IOptions<AuthenticationOptions> option, DeleteBatchFileCommand deleteFiles) : IHandler
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
        var userFiles = await db.Files.Where(fi => fi.Creator.Id == user.Id).ToListAsync();
        await deleteFiles.ExecuteAsync(userFiles.Select(fi => fi.Key));

        var chans = await db.Channels.Where(chan => chan.Members.Any(member => member.UserId == user.Id)
            && chan.Type == Channels.ChannelType.DirectMessage).ToListAsync();

        db.RemoveRange(chans);

        db.Users.Remove(user);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
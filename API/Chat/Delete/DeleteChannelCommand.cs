using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class DeleteChannelCommand(KeepGroupedDb db, IHubContext<ChatHub> hub) : IHandler
{
    public async Task<Result> ExecuteAsync(string id, User? sender)
    {
        if (sender is null)
        {
            return UserProblems.NotAuthenticated();
        }

        var channel = await db.Channels.SingleOrDefaultAsync(c => c.Id == id);
        if (channel is null)
        {
            return ChannelProblems.NotFound(id);
        }

        db.Channels.Remove(channel);
        await db.SaveChangesAsync();

        var users = await db.Users.Where(user => user.IsOnline).Select(user => user.Id).ToListAsync();
        await hub.Clients.Users(users).SendAsync("RemoveChannel", channel.Id);

        return Result.OK;
    }
}

using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class DeleteMessageCommand(KeepGroupedDb db, IHubContext<ChatHub> hub) : IHandler
{
    public async Task<Result> ExecuteAsync(string msgId, User? sender)
    {
        if (sender is null)
        {
            return UserProblems.NotAuthenticated();
        }

        var msg = await db.Messages.SingleOrDefaultAsync(m => m.Id == msgId);
        if (msg is null)
        {
            return MessageProblems.NotFound(msgId);
        }

        if (msg.SenderId != sender.Id)
        {
            return MessageProblems.NotSender();
        }

        db.Messages.Remove(msg);
        await db.SaveChangesAsync();

        var users = await db.Users.Where(user => user.IsOnline).Select(user => user.Id).ToListAsync();
        await hub.Clients.Users(users).SendAsync("RemoveMessage", msgId);

        return Result.OK;
    }
}

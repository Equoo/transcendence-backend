using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class UpdateMessageCommand(KeepGroupedDb db, IHubContext<ChatHub> hub) : IHandler
{
    public async Task<Result> ExecuteAsync(
        string channelId,
        string msgId,
        User? sender,
        UpdateMessageRequest req
    )
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

        msg.Content = req.Content;
        msg.EditAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var response = MessageResponse.FromEntity(msg);

        var users = await db
            .Users.Where(user => user.IsOnline && user.Id != sender.Id)
            .Select(user => user.Id)
            .ToListAsync();
        await hub.Clients.Users(users).SendAsync("UpdateMessage", channelId, msgId, response);

        return Result.OK;
    }
}

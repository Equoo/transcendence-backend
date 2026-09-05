using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class CreateMessageCommand(KeepGroupedDb db, IHubContext<ChatHub> hub) : IHandler
{
    public async Task<Result<MessageResponse>> ExecuteAsync(
        string channelId,
        User? sender,
        CreateMessageRequest req
    )
    {
        if (sender is null)
        {
            return UserProblems.NotAuthenticated();
        }

        var channel = await db.Channels.SingleOrDefaultAsync(c => c.Id == channelId);
        if (channel is null)
        {
            return ChannelProblems.NotFound(channelId);
        }

        var msg = new Message(sender, channel, req.Content, req.MessageReference);
        db.Messages.Add(msg);
        await db.SaveChangesAsync();

        var response = MessageResponse.FromEntity(msg);

        var users = await db
            .Users.Where(user => user.IsOnline && user.Id != sender.Id)
            .Select(user => user.Id)
            .ToListAsync();
        await hub.Clients.Users(users).SendAsync("NewMessage", channelId, response);

        return response;
    }
}

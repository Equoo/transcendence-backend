using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

[Authorize]
public class ChatHub : Hub
{
    private readonly KeepGroupedDb _db;

    public ChatHub(KeepGroupedDb db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine($"User connected: {userId}");

        var user = await _db.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
        if (user is not null)
        {
            user.IsOnline = true;
            await _db.SaveChangesAsync();
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine($"Disconnected: {Context.ConnectionId}");

        if (exception != null)
        {
            Console.WriteLine($"Disconnected due to error: {exception.Message}");
        }

        var user = await _db.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
        if (user != null)
        {
            user.IsOnline = false;
            await _db.SaveChangesAsync();
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task ChannelSend(string channelId, string content)
    {
        var senderId = Context.UserIdentifier;

        var sender = await _db.Users.Where(user => user.Id == senderId).FirstOrDefaultAsync();
        var channel = await _db.Channels.SingleOrDefaultAsync(e => e.Id == channelId);
        if (sender is null || channel is null)
            return;

        var msg = new Message(sender, channel, content);
        await _db.Messages.AddAsync(msg);
        await _db.SaveChangesAsync();

        var users = await _db
            .Users.Where(user => user.IsOnline)
            .Select(user => user.Id)
            .ToListAsync();

        await Clients.Users(users).SendAsync("ReceiveMessage", msg);
    }

    // public async Task UserSend(string userId, string content)
    // {
    //     var senderId = Context.UserIdentifier;
    //
    //     await Clients.User(userId).SendAsync("ReceiveMessage", senderId, msg);
    // }
}

public static class ChatEndpoint
{
    public static void MapChat(this IEndpointRouteBuilder app)
    {
        var channels = app.MapGroup("/channels");

        channels.MapGet(
            "/",
            async (KeepGroupedDb db) =>
            {
                var channels = await db.Channels.ToListAsync();

                return Results.Ok(channels.Select(x => x));
            }
        );

        channels.MapGet(
            "/{id}",
            async (string id, KeepGroupedDb db) =>
            {
                var channel = await db
                    .Channels.Where(channel => channel.Id == id)
                    .FirstOrDefaultAsync();

                return channel is null
                    ? Results.NotFound()
                    : Results.Ok(ChannelResponse.FromEntity(channel));
            }
        );

        channels.MapGet(
            "/{id}/messages",
            async (string id, KeepGroupedDb db) =>
            {
                var messages = await db.Messages.Where(msg => msg.ChannelId == id).ToListAsync();

                return messages is null ? Results.NotFound() : Results.Ok(messages);
            }
        );

        var messages = app.MapGroup("/messages");
    }
}

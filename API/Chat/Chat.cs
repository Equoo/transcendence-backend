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
        await base.OnConnectedAsync();
    }

    public async Task ChannelSend(string channelId, string message)
    {
        var ownerId = Context.UserIdentifier;

        var channel = await _db.Channels.SingleOrDefaultAsync(e => e.Id == channelId);
        if (channel is null)
            Results.NotFound();

        await Clients.Users(channel.Messages).SendAsync("ReceiveMessage", ownerId, message);
    }

    public async Task UserSend(string userId, string message)
    {
        var ownerId = Context.UserIdentifier;

        await Clients.User(userId).SendAsync("ReceiveMessage", "Server", message);
    }
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

        var messages = app.MapGroup("/messages");
    }
}

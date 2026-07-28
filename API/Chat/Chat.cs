using KeepGrouped.API.Middlewares;
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
}

public static class ChatEndpoint
{
    public static void MapChat(this IEndpointRouteBuilder app)
    {
        var channels = app.MapGroup("/channels").WithTags("Channels");

        channels
            .MapGet(
                "/",
                // [Authorize]
                async (KeepGroupedDb db) =>
                {
                    var channels = await db.Channels.ToListAsync();

                    return Results.Ok(channels.Select(ChannelResponse.FromEntity));
                }
            )
            .WithName("GetChannels")
            .WithDescription("Get all channels")
            .Produces<List<ChannelResponse>>(201);

        channels
            .MapPost(
                "/",
                // [Authorize]
                async (
                    TokenContext tk,
                    KeepGroupedDb db,
                    IHubContext<ChatHub> hubContext,
                    ChannelCreate req
                ) =>
                {
                    var sender = tk.User;
                    if (sender is null)
                        return Results.Unauthorized();

                    var channel = new Channel(req.Name, req.Topic);

                    await db.Channels.AddAsync(channel);
                    await db.SaveChangesAsync();

                    var response = ChannelResponse.FromEntity(channel);

                    var users = await db
                        .Users.Where(user => user.IsOnline)
                        .Select(user => user.Id)
                        .ToListAsync();
                    await hubContext.Clients.Users(users).SendAsync("NewChannel", response);

                    return Results.Created($"/channel/{channel.Id}", response);
                }
            )
            .WithName("CreateChannel")
            .WithDescription("Create a new channel")
            .Produces<ChannelResponse>(201)
            .Produces(401);

        channels
            .MapGet(
                "/{id}",
                // [Authorize]
                async (TokenContext tk, KeepGroupedDb db, string id) =>
                {
                    var channel = await db.Channels.FindAsync(id);
                    return channel is null
                        ? Results.NotFound()
                        : Results.Ok(ChannelResponse.FromEntity(channel));
                }
            )
            .WithName("GetChannelById")
            .WithDescription("Get a channel by Id")
            .Produces<ChannelResponse>(201)
            .Produces(404);

        channels
            .MapDelete(
                "/{id}",
                // [Authorize]
                async (
                    TokenContext tk,
                    KeepGroupedDb db,
                    IHubContext<ChatHub> hubContext,
                    string id
                ) =>
                {
                    var sender = tk.User;
                    if (sender is null)
                        return Results.Unauthorized();

                    var channel = await db.Channels.FindAsync(id);
                    if (channel is null)
                        return Results.NotFound();

                    db.Channels.Remove(channel);
                    await db.SaveChangesAsync();

                    var users = await db
                        .Users.Where(user => user.IsOnline)
                        .Select(user => user.Id)
                        .ToListAsync();
                    await hubContext.Clients.Users(users).SendAsync("RemoveChannel", channel.Id);

                    return Results.NoContent();
                }
            )
            .WithName("RemoveChannel")
            .WithDescription("Remove a channel by Id")
            .Produces(401)
            .Produces(404)
            .Produces(204);

        channels
            .MapGet(
                "/{id}/messages",
                // [Authorize]
                async (
                    TokenContext tk,
                    KeepGroupedDb db,
                    string id,
                    DateTime? before,
                    int take = 20
                ) =>
                {
                    var sender = tk.User;
                    if (sender is null)
                        return Results.Unauthorized();

                    var channel = await db.Channels.FindAsync(id);
                    if (channel is null)
                        return Results.NotFound();

                    var query = db.Messages.Include(m => m.Sender).Where(m => m.ChannelId == id);

                    if (before.HasValue)
                    {
                        query = query.Where(m => m.SentAt < before.Value);
                    }

                    var messages = await query
                        .OrderByDescending(m => m.SentAt)
                        .Take(take)
                        .Select(m => MessageResponse.FromEntity(m))
                        .ToListAsync();

                    messages.Reverse();

                    return messages is null ? Results.NotFound() : Results.Ok(messages);
                }
            )
            .WithName("GetChannelMessages")
            .WithDescription("Get channel last messages. Can be limit and get by time")
            .Produces<List<MessageResponse>>(201)
            .Produces(401)
            .Produces(404);

        channels
            .MapPost(
                "/{id}/messages",
                // [Authorize]
                async (
                    TokenContext tk,
                    KeepGroupedDb db,
                    IHubContext<ChatHub> hubContext,
                    string id,
                    MessageCreate req
                ) =>
                {
                    var sender = tk.User;
                    if (sender is null)
                        return Results.Unauthorized();

                    var channel = await db.Channels.FindAsync(id);
                    if (channel is null)
                        return Results.NotFound();

                    var msg = new Message(sender, channel, req.Content);
                    await db.Messages.AddAsync(msg);
                    await db.SaveChangesAsync();

                    var users = await db
                        .Users.Where(user => user.IsOnline)
                        .Select(user => user.Id)
                        .ToListAsync();
                    await hubContext.Clients.Users(users).SendAsync("NewMessage", msg);

                    var response = MessageResponse.FromEntity(msg);
                    return Results.Created($"/{id}/messages/{msg.Id}", response);
                }
            )
            .WithName("ChannelSendMessage")
            .WithDescription("Send message in channel")
            .Produces<MessageResponse>(201)
            .Produces(401)
            .Produces(404);

        channels
            .MapGet(
                "/{id}/messages/{msgId}",
                // [Authorize]
                async (KeepGroupedDb db, string id, string msgId) =>
                {
                    var msg = await db.Messages.FindAsync(msgId);
                    return msg is null
                        ? Results.NotFound()
                        : Results.Ok(MessageResponse.FromEntity(msg));
                }
            )
            .WithName("GetChannelMessageById")
            .WithDescription("Get message by Id")
            .Produces<MessageResponse>(201)
            .Produces(404);

        channels
            .MapDelete(
                "/{id}/messages/{msgId}",
                // [Authorize]
                async (
                    TokenContext tk,
                    KeepGroupedDb db,
                    IHubContext<ChatHub> hubContext,
                    string id,
                    string msgId
                ) =>
                {
                    var sender = tk.User;
                    if (sender is null)
                        return Results.Unauthorized();

                    var msg = await db.Messages.FindAsync(msgId);
                    if (msg is null)
                        return Results.NotFound();

                    if (msg.Sender != sender)
                        return Results.Unauthorized();

                    db.Messages.Remove(msg);
                    await db.SaveChangesAsync();

                    var users = await db
                        .Users.Where(user => user.IsOnline)
                        .Select(user => user.Id)
                        .ToListAsync();
                    await hubContext.Clients.Users(users).SendAsync("RemoveMessage", msgId);

                    return Results.NoContent();
                }
            )
            .WithName("RemoveChannelMessage")
            .WithDescription("Remove a channel message by Id")
            .Produces(401)
            .Produces(404)
            .Produces(204);

        channels
            .MapPost(
                "/{id}/messages/{msgId}/ack",
                // [Authorize]
                async (TokenContext tk, KeepGroupedDb db, string id, string msgId) =>
                {
                    var sender = tk.User;
                    if (sender is null)
                        return Results.Unauthorized();

                    var channel = await db.Channels.FindAsync(id);
                    if (channel is null)
                        return Results.NotFound();

                    var msg = await db.Messages.FindAsync(msgId);
                    if (msg is null)
                        return Results.NotFound();

                    sender.ChannelsAckMsg[id] = msgId;
                    return Results.NoContent();
                }
            )
            .WithName("ChannelMessageAcknoledge")
            .WithDescription("Change user read state to message given")
            .Produces(204)
            .Produces(401)
            .Produces(404);
    }
}

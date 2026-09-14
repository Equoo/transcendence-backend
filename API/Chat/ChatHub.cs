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

        var user = await _db.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
        user?.IsOnline = true;
        await _db.SaveChangesAsync();

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (exception != null)
        {
            Console.WriteLine($"Disconnected due to error: {exception.Message}");
        }

        var user = await _db.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
        user?.IsOnline = false;
        await _db.SaveChangesAsync();

        await base.OnDisconnectedAsync(exception);
    }
}

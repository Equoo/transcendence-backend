using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Activities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API;

[Authorize]
public class KeepGroupedHub(KeepGroupedDb db) : Hub
{

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        var user = await db.Users.Where(user => user.Id == userId).SingleOrDefaultAsync();
        user?.IsOnline = true;
        await db.SaveChangesAsync();
        await base.OnConnectedAsync();
    }

    public async Task ActivityReported(ActivityEnum activity)
    {
        var userId = Context.UserIdentifier;

        await Clients.Others.SendAsync("OtherActivityUpdated", userId, activity);
    }

    public async Task ReportActivityTo(string userId, ActivityEnum activity)
    {
        await Clients.User(userId).SendAsync("OtherActivityUpdated", Context.UserIdentifier, activity);
    }

    public async Task AskOthersActivity()
    {
        await Clients.Others.SendAsync("ReportActivityTo", Context.UserIdentifier);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (exception != null)
        {
            Console.WriteLine($"Disconnected due to error: {exception.Message}");
        }

        var user = await db.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
        await db.SaveChangesAsync();
        user?.IsOnline = false;
        await base.OnDisconnectedAsync(exception);
    }
}

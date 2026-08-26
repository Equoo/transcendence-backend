using System.Text.RegularExpressions;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed partial class CreateChannelCommand(KeepGroupedDb db, IHubContext<ChatHub> hub) : IHandler
{
    public async Task<Result<ChannelResponse>> ExecuteAsync(CreateChannelRequest req, User? sender)
    {
        if (sender is null)
        {
            return UserProblems.NotAuthenticated();
        }

        if (ChannelNameValidation().IsMatch(req.Name))
        {
            return ChannelProblems.NameInvalid();
        }

        if (await db.Channels.AnyAsync(c => c.Name == req.Name))
        {
            return ChannelProblems.NameAlreadyUsed(req.Name);
        }

        var channel = new Channel(req.Name, req.Topic);
        db.Channels.Add(channel);
        await db.SaveChangesAsync();

        var response = ChannelResponse.FromEntity(channel);

        var users = await db.Users.Where(user => user.IsOnline).Select(user => user.Id).ToListAsync();
        await hub.Clients.Users(users).SendAsync("NewChannel", response);

        return response;
    }

    [GeneratedRegex(@"[\s\p{Lu}$%^&*()+|~={}[\]:;<>?,.\/\\`´'""!@#]")]
    private static partial Regex ChannelNameValidation();
}

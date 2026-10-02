using KeepGrouped.API.Problems;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class DeleteMessageCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(string channelId, string msgId, User sender)
	{
		var channel = await db.Channels
			.AsNoTracking()
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.Include(c => c.Category)
			.SingleOrDefaultAsync(c => c.Id == channelId);
		if (channel is null)
			return ChannelProblems.NotFound(channelId);

		if (!channel.IsWhitelisted(sender))
			return MessageProblems.AccessNotAuthorized();

		var msg = await db.Messages.SingleOrDefaultAsync(m => m.Id == msgId);
		if (msg is null)
		{
			return MessageProblems.NotFound(msgId);
		}

		if (msg.SenderId != sender.Id
			&& !sender.Role.Permission.HasFlag(Perms.ManageMessages))
		{
			return MessageProblems.NotAuthorized();
		}

		db.Messages.Remove(msg);
		await db.SaveChangesAsync();

		var users = await db
			.Users.Include(u => u.Role)
			.Where(user => user.IsOnline && user.Id != sender.Id)
			.ToListAsync();
		var whitelisted = users
			.Where(channel.IsWhitelisted)
			.Select(user => user.Id);
		await hub.Clients.Users(whitelisted).SendAsync("RemoveMessage", channelId, msgId);

		return Result.OK;
	}
}

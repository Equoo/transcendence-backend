using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class UpdateMessageCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(
		string channelId,
		string msgId,
		User sender,
		UpdateMessageRequest req
	)
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


		var msg = await db
			.Messages.Include(m => m.Channel)
			.Include(m => m.Sender)
			.Include(m => m.MessageRef)
				.ThenInclude(r => r!.Sender)
			.SingleOrDefaultAsync(m => m.ChannelId == channelId && m.Id == msgId);
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
			.Users.Include(u => u.Role)
			.Where(user => user.IsOnline && user.Id != sender.Id)
			.ToListAsync();
		var whitelisted = users
			.Where(channel.IsWhitelisted)
			.Select(user => user.Id);
		await hub.Clients.Users(whitelisted).SendAsync("UpdateMessage", channelId, msgId, response);

		return Result.OK;
	}
}

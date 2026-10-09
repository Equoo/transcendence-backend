using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class CreateMessageCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<MessageResponse>> ExecuteAsync(
		string channelId,
		User sender,
		CreateMessageRequest req
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

		if (req.Content.Length > 8192)
		{
			return MessageProblems.TooLong();
		}

		var msgRef = req.MessageReference is null
			? null
			: await db
				.Messages.Include(m => m.Sender)
				.SingleOrDefaultAsync(m =>
					m.ChannelId == channelId && m.Id == req.MessageReference
				);

		var msg = new Message(sender, channel, req.Content, msgRef);
		db.Messages.Add(msg);
		await db.SaveChangesAsync();

		var response = MessageResponse.FromEntity(msg);

		var users = await db
			.Users.Include(u => u.Role)
			.Where(user => user.IsOnline && user.Id != sender.Id)
			.ToListAsync();
		var whitelisted = users
			.Where(channel.IsWhitelisted)
			.Select(user => user.Id);
		await hub.Clients.Users(whitelisted).SendAsync("NewMessage", channelId, response);

		return response;
	}
}

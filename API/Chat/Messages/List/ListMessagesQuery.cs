using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class ListMessagesQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<MessageResponse>>> ExecuteAsync(
		string channelId,
		DateTime? before,
		int take,
		User sender
	)
	{
		var channel = await db.Channels
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.SingleOrDefaultAsync(c => c.Id == channelId);
		if (channel is null)
		{
			return ChannelProblems.NotFound(channelId);
		}

		if (!channel.IsWhitelisted(sender))
			return MessageProblems.AccessNotAuthorized();

		var query = db
			.Messages.Include(m => m.Sender)
			.Include(m => m.MessageRef)
				.ThenInclude(r => r!.Sender)
			.Include(m => m.Channel)
			.Where(m => m.ChannelId == channelId);

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

		return messages;
	}
}

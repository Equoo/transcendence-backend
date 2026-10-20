using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class ListMessagesQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<MessageResponse>>> ExecuteAsync(
		string channelId,
		DateTime? before,
		DateTime? after,
		DateTime? around,
		int take,
		User sender
	)
	{
		var channel = await db.Channels
			.AsNoTracking()
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.Include(c => c.Category)
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
		var defaultQuery = query;
		var processedTake = take;

		if (before.HasValue)
		{
			query = query.Where(m => m.SentAt < before.Value);
		}
		else if (after.HasValue)
		{
			query = query.Where(m => m.SentAt > after.Value);
		}
		else if (around.HasValue)
		{
			query = query.Where(m => m.SentAt < around.Value);
			processedTake = (int)Math.Floor((double)take / 2);
		}

		var messages = await query
			.OrderByDescending(m => m.SentAt)
			.Take(processedTake)
			.Select(m => MessageResponse.FromEntity(m))
			.ToListAsync();

		if (around.HasValue)
		{
			var leftMessages = await defaultQuery
				.Where(m => m.SentAt >= around.Value)
				.OrderByDescending(m => m.SentAt)
				.Take(processedTake + take % 2)
				.Select(m => MessageResponse.FromEntity(m))
				.ToListAsync();

			messages.AddRange(leftMessages);
		}

		messages.Reverse();

		return messages;
	}
}

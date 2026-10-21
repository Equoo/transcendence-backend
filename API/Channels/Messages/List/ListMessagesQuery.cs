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
			return ChannelProblems.NotFound(channelId);

		if (!channel.IsWhitelisted(sender))
			return MessageProblems.AccessNotAuthorized();

		var messages = ListChannelMessages(channelId);

		var page = (before, after, around) switch
		{
			({ } cursor, _, _) => await ListBefore(messages, cursor, take),
			(_, { } cursor, _) => await ListAfter(messages, cursor, take),
			(_, _, { } cursor) => await ListAround(messages, cursor, take),
			_ => await ListLatest(messages, take),
		};

		return page.OrderBy(m => m.SentAt).ToList();
	}

	private IQueryable<Message> ListChannelMessages(string channelId) =>
		db.Messages
			.Include(m => m.Sender)
			.Include(m => m.MessageRef)
				.ThenInclude(r => r!.Sender)
			.Include(m => m.Channel)
			.Where(m => m.ChannelId == channelId);

	private static Task<List<MessageResponse>> ListLatest(IQueryable<Message> messages, int take) =>
		messages
			.OrderByDescending(m => m.SentAt)
			.Take(take)
			.Select(m => MessageResponse.FromEntity(m))
			.ToListAsync();

	private static Task<List<MessageResponse>> ListBefore(IQueryable<Message> messages, DateTime cursor, int take) =>
		ListLatest(messages.Where(m => m.SentAt < cursor), take);

	private static Task<List<MessageResponse>> ListAfter(IQueryable<Message> messages, DateTime cursor, int take) =>
		messages
			.Where(m => m.SentAt > cursor)
			.OrderBy(m => m.SentAt)
			.Take(take)
			.Select(m => MessageResponse.FromEntity(m))
			.ToListAsync();

	private static async Task<List<MessageResponse>> ListAround(IQueryable<Message> messages, DateTime cursor, int take)
	{
		var olderCount = take / 2;
		var newerCount = take - olderCount;

		var older = await ListBefore(messages, cursor, olderCount);
		var newer = await messages
			.Where(m => m.SentAt >= cursor)
			.OrderBy(m => m.SentAt)
			.Take(newerCount)
			.Select(m => MessageResponse.FromEntity(m))
			.ToListAsync();

		return [.. older, .. newer];
	}
}

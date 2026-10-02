using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class GetMessageQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<MessageResponse>> ExecuteAsync(string channelId, string msgId, User sender)
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
			.Messages.AsNoTracking()
			.Include(m => m.Sender)
			.Include(m => m.Channel)
			.Include(m => m.MessageRef)
				.ThenInclude(r => r!.Sender)
			.SingleOrDefaultAsync(m => m.Id == msgId);

		return msg is null ? MessageProblems.NotFound(msgId) : MessageResponse.FromEntity(msg);
	}
}

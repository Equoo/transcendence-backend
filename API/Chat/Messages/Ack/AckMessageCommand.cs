using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class AckMessageCommand(KeepGroupedDb db) : IHandler
{
	public async Task<Result> ExecuteAsync(string channelId, string msgId, User? sender)
	{
		if (sender is null)
		{
			return UserProblems.NotAuthenticated();
		}

		var channel = await db.Channels.SingleOrDefaultAsync(c => c.Id == channelId);
		if (channel is null)
		{
			return ChannelProblems.NotFound(channelId);
		}

		var msg = await db.Messages.SingleOrDefaultAsync(m => m.Id == msgId);
		if (msg is null)
		{
			return MessageProblems.NotFound(msgId);
		}

		var ack = await db.ChannelAcks.SingleOrDefaultAsync(ack =>
			ack.ChannelId == channelId && ack.UserId == sender.Id
		);

		if (ack is null)
		{
			db.ChannelAcks.Add(new ChannelAck(sender, channel, msg.SentAt));
		}
		else
		{
			ack.AckAt = msg.SentAt;
		}
		await db.SaveChangesAsync();

		return Result.OK;
	}
}

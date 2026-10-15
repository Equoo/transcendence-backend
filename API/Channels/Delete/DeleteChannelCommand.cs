using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class DeleteChannelCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(string id)
	{
		var channel = await db.Channels.SingleOrDefaultAsync(c => c.Id == id);
		if (channel is null)
		{
			return ChannelProblems.NotFound(id);
		}

		db.Channels.Remove(channel);
		await db.SaveChangesAsync();

		await hub.Clients.All.SendAsync("RemoveChannel", channel.Id);

		return Result.OK;
	}
}

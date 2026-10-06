using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class ListChannelsQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<ChannelResponse>>> ExecuteAsync()
	{
		var channels = await db.Channels
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.AsNoTracking().ToListAsync();

		return channels.Select(ChannelResponse.FromEntity).ToList();
	}
}

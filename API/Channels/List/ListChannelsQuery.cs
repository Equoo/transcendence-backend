using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class ListChannelsQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<ChannelResponse>>> ExecuteAsync()
	{
		var channels = await db.Channels
			.Where(c => c.Type == ChannelType.Text)
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.AsNoTracking().ToListAsync();

		return channels.Select(ChannelResponse.FromEntity).ToList();
	}
}

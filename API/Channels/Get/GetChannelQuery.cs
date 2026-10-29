using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class GetChannelQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<ChannelResponse>> ExecuteAsync(string id)
	{
		var channel = await db.Channels
			.Where(c => c.Id == id && c.Type == ChannelType.Text)
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.AsNoTracking()
			.SingleOrDefaultAsync(c => c.Id == id);

		return channel is null ? ChannelProblems.NotFound(id) : ChannelResponse.FromEntity(channel);
	}
}

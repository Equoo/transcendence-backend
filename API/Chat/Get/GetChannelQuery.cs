using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class GetChannelQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<ChannelResponse>> ExecuteAsync(string id)
	{
		var channel = await db.Channels
			.Where(c => c.Id == id)
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.Select(c => ChannelResponse.FromEntity(c))
			.AsNoTracking()
			.SingleOrDefaultAsync(c => c.Id == id);

		return channel is null ? ChannelProblems.NotFound(id) : channel;
	}
}

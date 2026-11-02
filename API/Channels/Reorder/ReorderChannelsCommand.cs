using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class ReorderChannelsCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<List<ChannelResponse>>> ExecuteAsync(ReorderChannelsRequest req)
	{
		if (req.Category is not null && !await db.ChannelCategories.AnyAsync(c => c.Id == req.Category))
		{
			return CategoryProblems.NotFound(req.Category);
		}

		var channels = await db.Channels
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.Where(c => c.Type == ChannelType.Text && c.EventId == null
				&& (req.Channels.Contains(c.Id) || c.CategoryId == req.Category))
			.ToDictionaryAsync(c => c.Id);

		if (req.Channels.Count != channels.Count
			|| req.Channels.Distinct().Count() != req.Channels.Count
			|| req.Channels.Any(id => !channels.ContainsKey(id)))
		{
			return ChannelProblems.OrderMismatch();
		}

		for (var i = 0; i < req.Channels.Count; i++)
		{
			var channel = channels[req.Channels[i]];
			channel.CategoryId = req.Category;
			channel.Order = (uint)i;
		}
		await db.SaveChangesAsync();

		var response = req.Channels
			.Select(id => ChannelResponse.FromEntity(channels[id]))
			.ToList();

		foreach (var channel in response)
		{
			await hub.Clients.All.SendAsync("UpdateChannel", channel);
		}

		return response;
	}
}

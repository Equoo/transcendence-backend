using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class ReorderCategoriesCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<List<ChannelCategoryResponse>>> ExecuteAsync(ReorderCategoriesRequest req)
	{
		var categories = await db.ChannelCategories
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.ToDictionaryAsync(c => c.Id);

		if (req.Categories.Count != categories.Count
			|| req.Categories.Distinct().Count() != req.Categories.Count
			|| req.Categories.Any(id => !categories.ContainsKey(id)))
		{
			return CategoryProblems.OrderMismatch();
		}

		for (var i = 0; i < req.Categories.Count; i++)
		{
			categories[req.Categories[i]].Order = (uint)i;
		}
		await db.SaveChangesAsync();

		var response = req.Categories
			.Select(id => ChannelCategoryResponse.FromEntity(categories[id]))
			.ToList();

		foreach (var category in response)
		{
			await hub.Clients.All.SendAsync("UpdateCategory", category);
		}

		return response;
	}
}

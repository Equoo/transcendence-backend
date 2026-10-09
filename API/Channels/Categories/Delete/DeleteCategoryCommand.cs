using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class DeleteCategoryCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(string id)
	{
		var category = await db.ChannelCategories.SingleOrDefaultAsync(c => c.Id == id);
		if (category is null)
		{
			return CategoryProblems.NotFound(id);
		}

		db.ChannelCategories.Remove(category);
		await db.SaveChangesAsync();

		await hub.Clients.All.SendAsync("RemoveCategory", category.Id);

		return Result.OK;
	}
}

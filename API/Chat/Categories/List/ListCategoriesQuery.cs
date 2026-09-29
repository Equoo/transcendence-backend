using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class ListCategoriesQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<ChannelCategoryResponse>>> ExecuteAsync()
	{
		var categories = await db.ChannelCategories
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.AsNoTracking().OrderBy(c => c.Order).ToListAsync();

		return categories.Select(ChannelCategoryResponse.FromEntity).ToList();
	}
}

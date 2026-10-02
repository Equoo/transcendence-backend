using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class GetCategoryQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<ChannelCategoryResponse>> ExecuteAsync(string id)
	{
		var category = await db.ChannelCategories.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);

		return category is null ? CategoryProblems.NotFound(id) : ChannelCategoryResponse.FromEntity(category);
	}
}

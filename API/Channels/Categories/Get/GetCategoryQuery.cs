using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class GetCategoryQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<ChannelCategoryResponse>> ExecuteAsync(string id)
	{
		var category = await db.ChannelCategories
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);

		return category is null ? CategoryProblems.NotFound(id) : ChannelCategoryResponse.FromEntity(category);
	}
}

using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class UpdateCategoryCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<ChannelCategoryResponse>> ExecuteAsync(string id, UpdateCategoryRequest req)
	{
		var category = await db.ChannelCategories
			.Include(c => c.RolesWhitelist)
				.ThenInclude(cr => cr.Role)
			.SingleOrDefaultAsync(c => c.Id == id);
		if (category is null)
		{
			return CategoryProblems.NotFound(id);
		}

		if (!ChannelSlug.IsValidName(req.Name, allowWhitespace: true))
		{
			return CategoryProblems.NameInvalid();
		}

		if (await db.ChannelCategories.AnyAsync(c => c.Name.ToLower() == req.Name.ToLower() && c.Id != id))
		{
			return CategoryProblems.NameAlreadyUsed(req.Name);
		}

		var res = await ChannelRole.UpdateRoles(db, req.WhitelistRoles, category, (c, role) => new ChannelRole
		{
			CategoryId = c.Id,
			RoleId = role.Id,
			Role = role
		});
		if (res.IsProblem)
			return res.Problem;

		category.Name = req.Name;
		category.Order = req.Order;
		await db.SaveChangesAsync();

		var response = ChannelCategoryResponse.FromEntity(category);
		await hub.Clients.All.SendAsync("UpdateCategory", response);

		return response;
	}
}

using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class CreateCategoryCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<ChannelCategoryResponse>> ExecuteAsync(CreateCategoryRequest req)
	{
		if (!ChannelSlug.IsValidName(req.Name, allowWhitespace: true))
		{
			return CategoryProblems.NameInvalid();
		}

		if (await db.ChannelCategories.AnyAsync(c => c.Name.ToLower() == req.Name.ToLower()))
		{
			return CategoryProblems.NameAlreadyUsed(req.Name);
		}

		var category = new ChannelCategory(req.Name, req.Order);

		var res = await ChannelRole.UpdateRoles(db, req.WhitelistRoles, category, (c, role) => new ChannelRole
		{
			CategoryId = c.Id,
			RoleId = role.Id,
			Role = role
		});
		if (res.IsProblem)
			return res.Problem;

		db.ChannelCategories.Add(category);
		await db.SaveChangesAsync();

		var response = ChannelCategoryResponse.FromEntity(category);

		await hub.Clients.All.SendAsync("NewCategory", response);

		return response;
	}
}

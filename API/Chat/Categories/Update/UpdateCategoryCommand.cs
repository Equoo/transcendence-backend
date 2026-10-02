using System.Text.RegularExpressions;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed partial class UpdateCategoryCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(string id, UpdateCategoryRequest req, User? sender)
	{
		if (sender is null)
		{
			return UserProblems.NotAuthenticated();
		}

		var category = await db.ChannelCategories.SingleOrDefaultAsync(c => c.Id == id);
		if (category is null)
		{
			return CategoryProblems.NotFound(id);
		}

		if (CategoryNameValidation().IsMatch(req.Name))
		{
			return CategoryProblems.NameInvalid();
		}

		if (await db.ChannelCategories.AnyAsync(c => c.Name == req.Name && c.Id != id))
		{
			return CategoryProblems.NameAlreadyUsed(req.Name);
		}

		category.Name = req.Name;
		category.Order = req.Order;
		await db.SaveChangesAsync();

		await hub.Clients.All.SendAsync("UpdateCategory", ChannelCategoryResponse.FromEntity(category));

		return Result.OK;
	}

	[GeneratedRegex(@"[$%^&*()+|~={}[\]:;<>?,.\/\\`´'""!@#]")]
	private static partial Regex CategoryNameValidation();
}

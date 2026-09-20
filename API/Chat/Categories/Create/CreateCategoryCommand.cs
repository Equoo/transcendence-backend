using System.Text.RegularExpressions;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed partial class CreateCategoryCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<ChannelCategoryResponse>> ExecuteAsync(CreateCategoryRequest req, User? sender)
	{
		if (sender is null)
		{
			return UserProblems.NotAuthenticated();
		}

		if (CategoryNameValidation().IsMatch(req.Name))
		{
			return CategoryProblems.NameInvalid();
		}

		if (await db.ChannelCategories.AnyAsync(c => c.Name == req.Name))
		{
			return CategoryProblems.NameAlreadyUsed(req.Name);
		}

		var category = new ChannelCategory(req.Name, req.Order);
		db.ChannelCategories.Add(category);
		await db.SaveChangesAsync();

		var response = ChannelCategoryResponse.FromEntity(category);

		await hub.Clients.All.SendAsync("NewCategory", response);

		return response;
	}

	[GeneratedRegex(@"[$%^&*()+|~={}[\]:;<>?,.\/\\`´'""!@#]")]
	private static partial Regex CategoryNameValidation();
}

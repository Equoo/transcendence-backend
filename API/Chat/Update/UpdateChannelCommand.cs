using System.Text.RegularExpressions;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed partial class UpdateChannelCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(string id, UpdateChannelRequest req, User? sender)
	{
		if (sender is null)
		{
			return UserProblems.NotAuthenticated();
		}

		var channel = await db.Channels.SingleOrDefaultAsync(c => c.Id == id);
		if (channel is null)
		{
			return ChannelProblems.NotFound(id);
		}

		if (ChannelNameValidation().IsMatch(req.Name))
		{
			return ChannelProblems.NameInvalid();
		}

		if (await db.Channels.AnyAsync(c => c.Name == req.Name && c.Id != id))
		{
			return ChannelProblems.NameAlreadyUsed(req.Name);
		}

		if (req.Category is not null && !await db.ChannelCategories.AnyAsync(c => c.Id == req.Category))
		{
			return CategoryProblems.NotFound(req.Category);
		}

		channel.Name = req.Name;
		channel.Topic = req.Topic;
		channel.Category = req.Category;
		await db.SaveChangesAsync();

		await hub.Clients.All.SendAsync("UpdateChannel", ChannelResponse.FromEntity(channel));

		return Result.OK;
	}

	[GeneratedRegex(@"[\s\p{Lu}$%^&*()+|~={}[\]:;<>?,.\/\\`´'""!@#]")]
	private static partial Regex ChannelNameValidation();
}

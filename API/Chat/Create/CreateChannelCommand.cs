using System.Text.RegularExpressions;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed partial class CreateChannelCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<ChannelResponse>> ExecuteAsync(CreateChannelRequest req, User? sender)
	{
		if (sender is null)
		{
			return UserProblems.NotAuthenticated();
		}

		if (ChannelNameValidation().IsMatch(req.Name))
		{
			return ChannelProblems.NameInvalid();
		}

		if (await db.Channels.AnyAsync(c => c.EventId == null && c.Name == req.Name))
		{
			return ChannelProblems.NameAlreadyUsed(req.Name);
		}

		if (req.Category is not null && !await db.ChannelCategories.AnyAsync(c => c.Id == req.Category))
		{
			return CategoryProblems.NotFound(req.Category);
		}

		var channel = new Channel(req.Name, req.Topic, req.EventId) { Category = req.Category };
		db.Channels.Add(channel);
		await db.SaveChangesAsync();

		var response = ChannelResponse.FromEntity(channel);

		await hub.Clients.All.SendAsync("NewChannel", response);

		return response;
	}

	[GeneratedRegex(@"[\s\p{Lu}$%^&*()+|~={}[\]:;<>?,.\/\\`´'""!@#]")]
	private static partial Regex ChannelNameValidation();
}

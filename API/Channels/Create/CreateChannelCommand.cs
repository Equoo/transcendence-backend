using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Channels;

public sealed class CreateChannelCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<ChannelResponse>> ExecuteAsync(CreateChannelRequest req)
	{
		if (!ChannelSlug.IsValidName(req.Name))
		{
			return ChannelProblems.NameInvalid();
		}

		if (await db.Channels.AnyAsync(c => c.Type == ChannelType.Text && c.EventId == null && c.Name.ToLower() == req.Name.ToLower()))
		{
			return ChannelProblems.NameAlreadyUsed(req.Name);
		}

		if (req.Category is not null && !await db.ChannelCategories.AnyAsync(c => c.Id == req.Category))
		{
			return CategoryProblems.NotFound(req.Category);
		}

		var channel = new Channel(req.Name, req.Topic, null)
		{
			CategorySync = req.CategorySync,
			CategoryId = req.Category
		};

		var res = await ChannelRole.UpdateRoles(db, req.WhitelistRoles, channel, (c, role) => new ChannelRole
		{
			ChannelId = c.Id,
			RoleId = role.Id,
			Role = role
		});
		if (res.IsProblem)
			return res.Problem;

		db.Channels.Add(channel);
		await db.SaveChangesAsync();

		var response = ChannelResponse.FromEntity(channel);

		await hub.Clients.All.SendAsync("NewChannel", response);

		return response;
	}
}

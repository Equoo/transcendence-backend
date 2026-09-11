using KeepGrouped.API.Chat;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class CreateEventCommand(KeepGroupedDb db) : IHandler
{
	public async Task<Result<CreateEventResponse>> ExecuteAsync(CreateEventRequest req, User organizer)
	{
		List<EventRole> roles = await db.EventRoles.Where(er => req.EventRoleIds.Contains(er.Id)).ToListAsync();
		if (roles.Count != req.EventRoleIds.Distinct().Count())
		{
			return EventProblems.UnknownEventRoles(req.EventRoleIds.Except(roles.Select(er => er.Id)));
		}

		List<StorageFile> files = await db.Files.Where(fi => req.FileKeys.Contains(fi.Key)).ToListAsync();
		if (files.Count != req.FileKeys.Distinct().Count())
		{
			return EventProblems.UnknownFiles(req.FileKeys.Except(files.Select(fi => fi.Key)));
		}

		EventRole? anyRole = await db.EventRoles.SingleOrDefaultAsync(er => er.Name == EventRole.Implicit);
		if (anyRole is null)
		{
			return EventProblems.DefaultRoleMissing();
		}

		var ev = new Event
		{
			Name = req.Name,
			Date = req.Date.ToUniversalTime(),
			Size = req.Size,
			Location = req.Location,
			Description = req.Description,
			Organizer = organizer,
			EventRoles = roles,
			Files = files,
			Tags = req.Tags
		};

		var channelReq = new CreateChannelRequest { Name = ChannelSlug.Sanitize(req.Name), EventId = ev.Id };
		var channelResult = await new CreateChannelCommand(db, null).ExecuteAsync(channelReq, organizer);
		if (channelResult.IsProblem)
		{
			return channelResult.Problem!;
		}

		ev.ChannelId = channelResult.Value.Id;

		ev.EventRoles.Add(anyRole);
		db.Events.Add(ev);
		await db.SaveChangesAsync();

		return CreateEventResponse.FromEntity(ev);
	}
}

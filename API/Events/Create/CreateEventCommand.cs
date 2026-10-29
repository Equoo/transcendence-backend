using KeepGrouped.API.Channels;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class CreateEventCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub, RagQueue rag) : IHandler
{
    public async Task<Result<CreateEventResponse>> ExecuteAsync(CreateEventRequest req, User organizer)
    {
        string channelName = ChannelSlug.Sanitize(req.Name);
        if (channelName.Length == 0)
        {
            return EventProblems.NameInvalid();
        }

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
            ExpireAt = req.ExpireAt.ToUniversalTime(),
            Size = req.Size,
            Location = req.Location,
            Description = req.Description,
            Organizer = organizer,
            EventRoles = roles,
            Files = files,
            Tags = req.Tags
        };

        ev.Channel = new Channel(channelName, string.Empty, ev.Id);

        ev.EventRoles.Add(anyRole);
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        var channel = ChannelResponse.FromEntity(ev.Channel);
        await hub.Clients.All.SendAsync("NewChannel", channel);

        rag.Upsert(ev);
        return CreateEventResponse.FromEntity(ev, channel);
    }
}

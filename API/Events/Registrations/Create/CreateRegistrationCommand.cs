using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class CreateRegistrationCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string eventId, User? user, CreateRegistrationRequest req)
    {
        if (user is null)
        {
            return UserProblems.NotAuthenticated();
        }

        Event? ev = await db.Events
            .Include(e => e.EventRoles)
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .SingleOrDefaultAsync(e => e.Id == eventId);
        if (ev is null)
        {
            return EventProblems.NotFound(eventId);
        }

        EventRole? eventRole = await db.EventRoles.SingleOrDefaultAsync(er => er.Id == req.EventRoleId);
        if (eventRole is null)
        {
            return EventRoleProblems.NotFound(req.EventRoleId);
        }

        if (!ev.EventRoles.Any(er => er.Id == eventRole.Id))
        {
            return EventRoleProblems.NotOfferedByEvent(eventRole.Name, ev.Name);
        }

        if (ev.Registrations.Any(r => r.User.Id == user.Id))
        {
            return RegistrationProblems.AlreadyRegistered(user.UserName, ev.Name);
        }

        if (ev.Registrations.Count >= ev.Size)
        {
            return EventProblems.Full(ev.Name, ev.Size);
        }

        var registration = new Registration()
        {
            User = user,
            Role = eventRole
        };

        ev.Registrations.Add(registration);

        await db.SaveChangesAsync();

        return Result.Success;
    }
}

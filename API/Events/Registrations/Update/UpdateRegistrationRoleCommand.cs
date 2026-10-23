using KeepGrouped.API.Problems;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class UpdateRegistrationRoleCommand(KeepGroupedDb db, RagQueue rag) : IHandler
{
    public async Task<Result> ExecuteAsync(string eventId, UpdateRegistrationRoleRequest req)
    {
        Event? ev = await db.Events
            .Include(e => e.EventRoles)
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .Include(e => e.Registrations).ThenInclude(r => r.Role)
            .SingleOrDefaultAsync(e => e.Id == eventId);
        if (ev is null)
        {
            return EventProblems.NotFound(eventId);
        }

        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == req.UserId);
        if (user is null)
        {
            return UserProblems.NotFound(req.UserId);
        }

        Registration? registration = ev.Registrations.SingleOrDefault(r => r.User.Id == user.Id);
        if (registration is null)
        {
            return RegistrationProblems.NotRegistered(user.UserName, ev.Name);
        }

        EventRole? eventRole = ev.EventRoles.SingleOrDefault(er => er.Id == req.EventRoleId);
        if (eventRole is null)
        {
            EventRole? existing = await db.EventRoles.SingleOrDefaultAsync(er => er.Id == req.EventRoleId);
            if (existing is null)
            {
                return EventRoleProblems.NotFound(req.EventRoleId);
            }

            return EventRoleProblems.NotOfferedByEvent(existing.Name, ev.Name);
        }

        registration.Role = eventRole;

        await db.SaveChangesAsync();
        rag.Upsert(RagItem.Event(ev.Id));
        return Result.OK;
    }
}

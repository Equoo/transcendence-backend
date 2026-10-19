using KeepGrouped.API.Problems;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class DeleteUserRegistrationCommand(KeepGroupedDb db, RagQueue rag) : IHandler
{
    public async Task<Result> ExecuteAsync(string eventId, string userId)
    {
        Event? ev = await db.Events
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .SingleOrDefaultAsync(e => e.Id == eventId);
        if (ev is null)
        {
            return EventProblems.NotFound(eventId);
        }

        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return UserProblems.NotFound(userId);
        }

        Registration? registration = ev.Registrations.SingleOrDefault(r => r.User.Id == user.Id);
        if (registration is null)
        {
            return RegistrationProblems.NotRegistered(user.UserName, ev.Name);
        }

        ev.Registrations.Remove(registration);
        await db.SaveChangesAsync();
        rag.Upsert(RagItem.Event(ev.Id));
        return Result.OK;
    }
}

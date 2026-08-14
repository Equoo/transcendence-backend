using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class DeleteRegistrationCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string eventId, User? user)
    {
        if (user is null)
        {
            return UserProblems.NotAuthenticated();
        }

        Event? ev = await db.Events
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .SingleOrDefaultAsync(e => e.Id == eventId);
        if (ev is null)
        {
            return EventProblems.NotFound(eventId);
        }

        Registration? registration = ev.Registrations.SingleOrDefault(r => r.User.Id == user.Id);
        if (registration is null)
        {
            return RegistrationProblems.NotRegistered(user.UserName, ev.Name);
        }

        ev.Registrations.Remove(registration);
        await db.SaveChangesAsync();
        return Result.Success;
    }
}

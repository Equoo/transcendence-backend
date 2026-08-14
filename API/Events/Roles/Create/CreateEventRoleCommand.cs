using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Events;

public sealed class CreateEventRoleCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result<CreateEventRoleResponse>> ExecuteAsync(CreateEventRoleRequest req)
    {
        if (req.Name == EventRole.Implicit)
        {
            return EventRoleProblems.Reserved(req.Name);
        }

        if (await db.EventRoles.AnyAsync(er => er.Name == req.Name))
        {
            return EventRoleProblems.AlreadyExists(req.Name);
        }

        EventRole er = new() { Name = req.Name };

        db.EventRoles.Add(er);
        await db.SaveChangesAsync();

        return CreateEventRoleResponse.FromEntity(er);
    }
}

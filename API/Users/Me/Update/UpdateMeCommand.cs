using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public sealed class UpdateMeCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result<UpdateMeResponse>> ExecuteAsync(User user, UpdateMeRequest req)
    {
        if (await db.Users.AnyAsync(u => u.UserName == req.UserName && u.Id != user.Id))
        {
            return UserProblems.NameAlreadyUsed(req.UserName);
        }

        user.UserName = req.UserName;

        await db.SaveChangesAsync();

        return UpdateMeResponse.FromEntity(user);
    }
}

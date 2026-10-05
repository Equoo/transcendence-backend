using KeepGrouped.API.Problems;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class UpdateMeCommand(KeepGroupedDb db, IOptions<AuthenticationOptions> option) : IHandler
{
    public async Task<Result<UpdateMeResponse>> ExecuteAsync(User user, UpdateMeRequest req)
    {
        if (option.Value.DefaultAdminLogin == user.UserName)
        {
            return UserProblems.UsernameCannotBeChanged();
        }

        if (await db.Users.AnyAsync(u => u.UserName == req.UserName && u.Id != user.Id))
        {
            return UserProblems.NameAlreadyUsed(req.UserName);
        }

        user.UserName = req.UserName;

        await db.SaveChangesAsync();

        return UpdateMeResponse.FromEntity(user);
    }
}

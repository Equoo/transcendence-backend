
using KeepGrouped.API;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class DeleteUserQuery(KeepGroupedDb db, IOptions<AuthenticationOptions> option) : IHandler
{
    public async Task<Result> ExecuteAsync(string id)
    {
        User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return UserProblems.NotFound(id);
        }

        if (option.Value.DefaultAdminLogin == user.UserName)
        {
            return UserProblems.UserCannotBeDelete();
        }

        RefreshToken? refresh = await db.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == id);

        if (refresh is not null)
        {
            db.RefreshTokens.Remove(refresh);
        }

        db.Users.Remove(user);

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
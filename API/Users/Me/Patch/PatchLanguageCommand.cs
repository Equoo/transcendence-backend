using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

public sealed class PatchLanguageCommand(KeepGroupedDb db, TokenContext tk) : IHandler
{
    public async Task<Result> ExecuteAsync(LanguageEnum language)
    {
        if (!Enum.IsDefined(language))
        {
            return UserProblems.InvalidLanguage(language);
        }

        User? db_user = await db.Users.SingleOrDefaultAsync(u => u == tk.User);

        if (db_user is null)
        {
            return UserProblems.NotFound(tk.User.Id);
        }

        db_user.Language = language;

        await db.SaveChangesAsync();

        return Result.OK;
    }
}
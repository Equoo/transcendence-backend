using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

public static class PatchLanguageEndpoint
{
    public async static void MapPatchLanguage(IEndpointRouteBuilder me)
    {
        me.MapPatch("/language", [Authorize] async (PatchLanguageCommand command, LanguageEnum language) =>
        {
            var res = await command.ExecuteAsync(language);

            if (res.IsProblem)
            {
                return res.Problem;
            }

            return Results.Ok();
        });
    }
}
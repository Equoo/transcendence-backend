using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public static class PatchLanguageEndpoint
{
    public async static void MapPatchLanguage(this IEndpointRouteBuilder me)
    {
        me.MapPatch("/language", [Authorize] async (PatchLanguageCommand command, [FromBody] LanguageEnum Language) =>
        {
            var res = await command.ExecuteAsync(Language);

            if (res.IsProblem)
            {
                return res.Problem;
            }

            return Results.Ok();
        });
    }
}
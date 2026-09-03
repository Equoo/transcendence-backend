using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Users;

public static class PatchAvatarEndpoint
{
    public static void MapPatchAvatar(this IEndpointRouteBuilder group)
    {
        group.MapPatch("/avatar", [Authorize] async (PatchAvatarCommand Cmd, IFormFile File, TokenContext tk) =>
        {
            var res = await Cmd.ExecuteAsync(File, tk.User);
            if (res.IsProblem)
            {
                return res.Problem;
            }

            return Results.Ok(res.Value);
        })
        .DisableAntiforgery()
        .WithName("me.avatar.patch")
        .WithSummary("Change the current user's avatar")
        .WithDescription("Uploads an image as `multipart/form-data` (field `file`), sets it as the current user's avatar and deletes the previous one, if any.")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<GetFileMetaResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status502BadGateway);
    }
}

using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

public record UpdateMePassRequest
{
    [Required]
    public string Password { get; init; } = null!;

    [Required]
    public string NewPassword { get; init; } = null!;
}

public static class UpdateMePassEndpoint
{
    public static void MapUpdateMePass(this IEndpointRouteBuilder me)
    {
        me.MapPatch("/password", [Authorize] async (UpdateMePassRequest req, UpdateMePassCommand cmd, LogoutQuery query, TokenContext tk, HttpContext http) =>
        {

            var result = await cmd.ExecuteAsync(req);

            if (result.IsProblem)
            {
                return result.Problem;
            }


            var res = await query.ExecAsync(tk.User.Id);
            if (res.IsProblem)
            {
                return result.Problem;
            }

            TokenCookies.Remove(http);


            return Results.Ok();
        });
    }
}
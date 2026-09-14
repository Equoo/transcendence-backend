
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Users;

public static class ResetPasswordEndpoint
{
    public static void MapResetPassword(this IEndpointRouteBuilder auth)
    {
        auth.MapPatch("{id}/password", [Authorize] async (ResetPasswordQuery pass, LogoutQuery logout,  string id, [FromBody] string password) =>
        {
            var result = await pass.ExecAsync(id, password);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            // var reset = await logout.ExecAsync(id);
            // if (reset.IsProblem)
            // {
            //     return reset.Problem;
            // }

            return Results.Ok();
        });
    }
} 
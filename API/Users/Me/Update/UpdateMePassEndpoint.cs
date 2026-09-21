
using System.ComponentModel.DataAnnotations;
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
        me.MapPatch("/password", [Authorize] async (UpdateMePassRequest req, UpdateMePassCommand cmd) =>
        {
            var result = await cmd.ExecuteAsync(req);

            if (result.IsProblem)
            {
                return result.Problem;
            }
            return Results.Ok();
        });
    }
}
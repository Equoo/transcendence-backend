using Microsoft.AspNetCore.Authorization;

public static class DeleteAvatarEndpoint
{
    public static void MapDeleteAvatar(this IEndpointRouteBuilder me)
    {
        me.MapDelete("/avatar", [Authorize] async (DeleteAvatarCommand cmd) =>
        {
            var result = await cmd.ExecuteAsync();

            if (result.IsProblem)
            {
                return result.Problem;
            }
            return Results.Ok();
        });
    }
}
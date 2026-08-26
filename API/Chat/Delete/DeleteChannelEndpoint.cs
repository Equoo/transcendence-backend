using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class DeleteChannelEndpoint
{
    public static void MapDeleteChannel(this IEndpointRouteBuilder channels)
    {
        channels.MapDelete("/{id}", [Authorize] async (DeleteChannelCommand command, string id, TokenContext token) =>
        {
            var result = await command.ExecuteAsync(id, token.User);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("channels.delete")
        .WithSummary("Delete a channel")
        .WithDescription("Deletes a channel and every message it contains. Online users are notified of the removal.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

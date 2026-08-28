using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class DeleteMessageEndpoint
{
    public static void MapDeleteMessage(this IEndpointRouteBuilder messages)
    {
        messages
            .MapDelete(
                "/{msgId}",
                [Authorize]
                async (
                    DeleteMessageCommand command,
                    string channelId,
                    string msgId,
                    TokenContext token
                ) =>
                {
                    var result = await command.ExecuteAsync(channelId, msgId, token.User);
                    if (result.IsProblem)
                    {
                        return result.Problem;
                    }

                    return Results.NoContent();
                }
            )
            .WithName("messages.delete")
            .WithSummary("Delete a message")
            .WithDescription(
                "Deletes a message. Only the message's sender can delete it. Online users are notified of the removal."
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

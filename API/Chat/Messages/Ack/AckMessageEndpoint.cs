using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class AckMessageEndpoint
{
    public static void MapAckMessage(this IEndpointRouteBuilder messages)
    {
        messages.MapPost("/{msgId}/ack", [Authorize] async (AckMessageCommand command, string id, string msgId, TokenContext token) =>
        {
            var result = await command.ExecuteAsync(id, msgId, token.User);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("messages.ack")
        .WithSummary("Acknowledge a message")
        .WithDescription("Marks the message as read by the current user, updating their read state for the channel.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

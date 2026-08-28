using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public record UpdateMessageRequest
{
    [Required]
    public string Content { get; init; } = null!;
}

public static class UpdateMessageEndpoint
{
    public static void MapUpdateMessage(this IEndpointRouteBuilder messages)
    {
        messages
            .MapPut(
                "/{msgId}",
                [Authorize]
                async (
                    UpdateMessageCommand command,
                    string id,
                    string msgId,
                    TokenContext token,
                    UpdateMessageRequest req
                ) =>
                {
                    var result = await command.ExecuteAsync(id, msgId, token.User, req);
                    if (result.IsProblem)
                    {
                        return result.Problem;
                    }

                    return Results.NoContent();
                }
            )
            .WithName("messages.update")
            .WithSummary("Edit a message")
            .WithDescription(
                "Replaces the content of a message. Only the message's sender can edit it."
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

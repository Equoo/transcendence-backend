using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public record CreateMessageRequest
{
    [Required]
    public string Content { get; init; } = null!;
    public string? MessageReference { get; init; } = null;
}

public static class CreateMessageEndpoint
{
    public static void MapCreateMessage(this IEndpointRouteBuilder messages)
    {
        messages
            .MapPost(
                "/",
                [Authorize]
                async (
                    CreateMessageCommand command,
                    string id,
                    TokenContext token,
                    CreateMessageRequest req
                ) =>
                {
                    var result = await command.ExecuteAsync(id, token.User, req);
                    if (result.IsProblem)
                    {
                        return result.Problem;
                    }

                    return Results.CreatedAtRoute(
                        "messages.get",
                        new { id, msgId = result.Value.Id },
                        result.Value
                    );
                }
            )
            .WithName("messages.create")
            .WithSummary("Send a message")
            .WithDescription(
                "Sends a message from the current user to the channel. Online users other than the sender are notified of the new message."
            )
            .Produces<MessageResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

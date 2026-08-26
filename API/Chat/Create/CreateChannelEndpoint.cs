using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public record CreateChannelRequest
{
    [Required]
    [Length(1, 25)]
    public string Name { get; init; } = null!;
    [Length(0, 255)]
    public string Topic { get; init; } = string.Empty;
    public string? EventId { get; init; } = null;
}

public static class CreateChannelEndpoint
{
    public static void MapCreateChannel(this IEndpointRouteBuilder channels)
    {
        channels.MapPost("/", [Authorize] async (CreateChannelCommand command, CreateChannelRequest req, TokenContext token) =>
        {
            var result = await command.ExecuteAsync(req, token.User);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.CreatedAtRoute("channels.get", new { id = result.Value.Id }, result.Value);
        })
        .WithName("channels.create")
        .WithSummary("Create a channel")
        .WithDescription("Creates a new channel. The name must be unique, lowercase, and free of spaces or special characters. Online users are notified of the new channel.")
        .Produces<ChannelResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }
}

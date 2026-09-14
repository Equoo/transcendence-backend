using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class GetMessageEndpoint
{
    public static void MapGetMessage(this IEndpointRouteBuilder messages)
    {
        messages.MapGet("/{msgId}", [Authorize] async (GetMessageQuery query, string msgId) =>
        {
            var result = await query.ExecuteAsync(msgId);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("messages.get")
        .WithSummary("Get a message")
        .WithDescription("Returns a single message identified by its id.")
        .Produces<MessageResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

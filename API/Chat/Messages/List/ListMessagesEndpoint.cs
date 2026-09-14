using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class ListMessagesEndpoint
{
    public static void MapListMessages(this IEndpointRouteBuilder messages)
    {
        messages.MapGet("/", [Authorize] async (ListMessagesQuery query, string id, DateTime? before, int take = 20) =>
        {
            var result = await query.ExecuteAsync(id, before, take);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("messages.list")
        .WithSummary("List the messages of a channel")
        .WithDescription("Returns the last messages of the channel in chronological order. Can be limited with `take` and paged backwards with `before`.")
        .Produces<IEnumerable<MessageResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

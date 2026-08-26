using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class GetChannelEndpoint
{
    public static void MapGetChannel(this IEndpointRouteBuilder channels)
    {
        channels.MapGet("/{id}", [Authorize] async (GetChannelQuery query, string id) =>
        {
            var result = await query.ExecuteAsync(id);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("channels.get")
        .WithSummary("Get a channel")
        .WithDescription("Returns a single channel identified by its id.")
        .Produces<ChannelResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

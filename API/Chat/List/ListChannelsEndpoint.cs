using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Chat;

public static class ListChannelsEndpoint
{
    public static void MapListChannels(this IEndpointRouteBuilder channels)
    {
        channels.MapGet("/", [Authorize] async (ListChannelsQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("channels.list")
        .WithSummary("List channels")
        .WithDescription("Returns every channel.")
        .Produces<IEnumerable<ChannelResponse>>(StatusCodes.Status200OK);
    }
}

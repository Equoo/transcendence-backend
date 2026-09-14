namespace KeepGrouped.API.Chat;

public static class ChannelEndpoints
{
    public static void MapChannels(this IEndpointRouteBuilder app)
    {
        var channels = app.MapGroup("/channels").WithTags("Channels");

        channels.MapCreateChannel();
        channels.MapListChannels();
        channels.MapGetChannel();
        channels.MapDeleteChannel();
    }
}

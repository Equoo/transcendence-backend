namespace KeepGrouped.API.Chat;

public static class MessageEndpoints
{
    public static void MapMessages(this IEndpointRouteBuilder app)
    {
        var messages = app.MapGroup("/channels/{id}/messages").WithTags("Messages");

        messages.MapCreateMessage();
        messages.MapListMessages();
        messages.MapGetMessage();
        messages.MapUpdateMessage();
        messages.MapDeleteMessage();
        messages.MapAckMessage();
    }
}

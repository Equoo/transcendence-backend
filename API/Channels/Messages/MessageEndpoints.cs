namespace KeepGrouped.API.Channels;

public static class MessageEndpoints
{
	public static void MapMessages(this IEndpointRouteBuilder app)
	{
		var messages = app.MapGroup("/channels/{id}/messages").WithTags("Messages");

		messages.MapListMessages();
		messages.MapGetMessage();
		messages.MapAckMessage();
	}
}

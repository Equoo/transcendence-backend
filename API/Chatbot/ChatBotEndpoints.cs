
public static class ChatBotEndpoints
{
	public static void MapChatBot(this IEndpointRouteBuilder app)
	{
		var chatBot = app.MapGroup("/chatbot").WithTags("ChatBot");

	}
}
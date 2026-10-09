namespace KeepGrouped.API.Users.Me.Conversations;

public static class ConversationEndpoints
{
	public static void MapConversations(this IEndpointRouteBuilder me)
	{
		var conversations = me.MapGroup("/conversations").WithTags("Conversations");

		conversations.MapListConversations();
		conversations.MapCreateConversation();
		conversations.MapGetConversation();
		conversations.MapDeleteConversation();
	}
}


namespace KeepGrouped.API.AiBackend.Chatbot;

public class ChatRequest
{
	public string Message { get; set; } = null!;
}

public class AiChatRequest
{
	public string UserId { get; set; } = null!;
	public string Message { get; set; } = null!;
}
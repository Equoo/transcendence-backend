using KeepGrouped.API.Users;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Chatbot;


public interface IChatClient : IDisposable
{
	Task<MessageResponse> SendMessageAsync(string channelId, string content, string? messageRefId = null);
	Task<MessageResponse> EditMessageAsync(string messageId, string newContent);
	Task DeleteMessageAsync(string messageId);
	Task<MessageResponse> GetMessageAsync(string messageId);
	Task<IEnumerable<MessageResponse>> GetMessagesAsync(string channelId, int limit = 50, string? beforeMessageId = null);
}
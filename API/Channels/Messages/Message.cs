using KeepGrouped.API.Users;

namespace KeepGrouped.API.Channels;

public class Message
{
	public Message() { }

	public Message(User sender, Channel channel, string content, Message? messageRef)
	{
		Content = content;
		SenderId = sender.Id;
		Sender = sender;
		ChannelId = channel.Id;
		Channel = channel;
		MessageRefId = messageRef?.Id;
		MessageRef = messageRef;
		HasMessageRef = messageRef != null;
	}

	public string Id { get; init; } = Guid.NewGuid().ToString();
	public string Content { get; set; } = null!;
	public DateTime SentAt { get; } = DateTime.UtcNow;
	public DateTime? EditAt { get; set; } = null;

	public string? MessageRefId { get; set; } = null;
	public Message? MessageRef { get; set; } = null;
	public bool HasMessageRef { get; set; } = false;

	public string SenderId { get; init; } = null!;
	public User Sender { get; init; } = null!;

	public string ChannelId { get; init; } = null!;
	public Channel Channel { get; init; } = null!;
}

public record MessageReference(string Id, string Content, DateTime SentAt, GetUserResponse Sender)
{
	public static MessageReference FromEntity(Message msg) =>
		new(msg.Id, msg.Content, msg.SentAt, GetUserResponse.FromEntity(msg.Sender));
}

public record MessageResponse(
	string Id,
	string Content,
	DateTime SentAt,
	DateTime? EditAt,
	MessageReference? MessageRef,
	bool HasMessageRef,
	GetUserResponse Sender,
	ChannelResponse Channel
)
{
	public static MessageResponse FromEntity(Message msg) =>
		new(
			msg.Id,
			msg.Content,
			msg.SentAt,
			msg.EditAt,
			(msg.MessageRefId is not null && msg.MessageRef is not null)
				? MessageReference.FromEntity(msg.MessageRef)
				: null,
			msg.HasMessageRef,
			GetUserResponse.FromEntity(msg.Sender),
			ChannelResponse.FromEntity(msg.Channel)
		);
}

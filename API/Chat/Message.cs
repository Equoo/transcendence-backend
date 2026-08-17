using KeepGrouped.API.Users;

namespace KeepGrouped.API.Chat;

public class Message
{
    public Message() { }

    public Message(User sender, Channel channel, string content)
    {
        Content = content;
        SenderId = sender.Id;
        Sender = sender;
        ChannelId = channel.Id;
        Channel = channel;
    }

    public string Id { get; } = Guid.NewGuid().ToString();
    public string Content { get; init; } = null!;
    public DateTime SentAt { get; } = DateTime.UtcNow;

    public string SenderId { get; init; } = null!;
    public User Sender { get; init; } = null!;

    public string ChannelId { get; init; } = null!;
    public Channel Channel { get; init; } = null!;
}

public record MessageResponse(
    string Id,
    string Content,
    DateTime SentAt,
    UserResponse Sender,
    ChannelResponse Channel
)
{
    public static MessageResponse FromEntity(Message msg) =>
        new(
            msg.Id,
            msg.Content,
            msg.SentAt,
            UserResponse.FromEntity(msg.Sender),
            ChannelResponse.FromEntity(msg.Channel)
        );
}

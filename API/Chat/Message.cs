using System.ComponentModel.DataAnnotations;
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

public record MessageUserResponse(string Id, string UserName)
{
    public static MessageUserResponse FromEntity(User u) => new(u.Id, u.UserName ?? "Unknown");
}

public record MessageResponse(
    string Id,
    string Content,
    DateTime SentAt,
    MessageUserResponse Sender,
    ChannelResponse Channel
)
{
    public static MessageResponse FromEntity(Message msg) =>
        new(
            msg.Id,
            msg.Content,
            msg.SentAt,
            MessageUserResponse.FromEntity(msg.Sender),
            ChannelResponse.FromEntity(msg.Channel)
        );
}

public record MessageCreate
{
    [Required]
    public string Content { get; init; } = null!;
}

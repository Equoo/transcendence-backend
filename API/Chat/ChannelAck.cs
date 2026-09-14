using KeepGrouped.API.Users;

namespace KeepGrouped.API.Chat;

public class ChannelAck
{
	public ChannelAck() { }

	public ChannelAck(User user, Channel channel, DateTime ack)
	{
		User = user;
		UserId = user.Id;
		Channel = channel;
		ChannelId = channel.Id;
		AckAt = ack;
	}

	public string UserId { get; init; } = null!;
	public User User { get; init; } = null!;
	public string ChannelId { get; init; } = null!;
	public Channel Channel { get; init; } = null!;
	public DateTime AckAt { get; set; } = DateTime.UtcNow;
}

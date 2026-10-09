using KeepGrouped.API.Users;

namespace KeepGrouped.API.Channels;

public class ChannelMember
{
	public string ChannelId { get; set; } = null!;
	public Channel Channel { get; set; } = null!;

	public string UserId { get; set; } = null!;
	public User User { get; set; } = null!;
}

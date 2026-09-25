using KeepGrouped.API.Roles;

namespace KeepGrouped.API.Chat;

public class ChannelRole
{
	public string? ChannelId { get; init; } = null;
	public Channel? Channel { get; init; } = null;
	public string? CategoryId { get; init; } = null;
	public ChannelCategory? Category { get; init; } = null;
	public string RoleId { get; init; } = null!;
	public Role Role { get; init; } = null!;
}

public record ChannelRoleResponse(string Id, string Name)
{
	public static ChannelRoleResponse FromEntity(string id, string name) => new(id, name);
}

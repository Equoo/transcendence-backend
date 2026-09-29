namespace KeepGrouped.API.Chat;

public class ChannelCategory : IRoleWhitelist
{
	public ChannelCategory() { }

	public ChannelCategory(string name, uint order)
	{
		Name = name;
		Order = order;
	}

	public string Id { get; init; } = Guid.NewGuid().ToString();
	public string Name { get; set; } = null!;
	public uint Order { get; set; } = 0;
	public ICollection<ChannelRole> RolesWhitelist { get; set; } = [];
}

public record ChannelCategoryResponse(
		string Id,
		string Name,
		uint Order,
		IReadOnlyList<ChannelRoleResponse> RolesWhitelist
)
{
	public static ChannelCategoryResponse FromEntity(ChannelCategory c) =>
		new(
				c.Id,
				c.Name,
				c.Order,
				[.. c.RolesWhitelist.Select((r) => new ChannelRoleResponse(r.Role.Id, r.Role.Name))]
			);
}

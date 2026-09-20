namespace KeepGrouped.API.Chat;

public class ChannelCategory
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
}

public record ChannelCategoryResponse(string Id, string Name, uint Order)
{
	public static ChannelCategoryResponse FromEntity(ChannelCategory c) =>
		new(c.Id, c.Name, c.Order);
}

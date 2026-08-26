namespace KeepGrouped.API.Chat;

public class Channel
{
    public Channel() { }

    public Channel(string name, string topic)
    {
        Name = name;
        Topic = topic;
    }

    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = null!;
    public string Topic { get; init; } = null!;
    public uint Order { get; init; } = 0;
    public DateTime CreateAt { get; } = DateTime.UtcNow;
    public string? EventId { get; } = null;
    public string? Category { get; } = null;
}

public record ChannelResponse(
    string Id,
    string Name,
    string Topic,
    DateTime CreateAt,
    string? EventId,
    string? Category
)
{
    public static ChannelResponse FromEntity(Channel c) =>
        new(c.Id, c.Name, c.Topic, c.CreateAt, c.EventId, c.Category);
}

using System.Text.RegularExpressions;
using KeepGrouped.API.Events;

namespace KeepGrouped.API.Chat;

public class Channel
{
	public Channel() { }

	public Channel(string name, string topic, string? eventId)
	{
		Name = name;
		Topic = topic;
		EventId = eventId;
	}

	public string Id { get; init; } = Guid.NewGuid().ToString();
	public string Name { get; init; } = null!;
	public string Topic { get; init; } = null!;
	public uint Order { get; init; } = 0;
	public DateTime CreateAt { get; } = DateTime.UtcNow;
	public string? Category { get; init; } = null;
	public string? EventId { get; set; } = null;
	public Event? Event { get; set; } = null;
}

public record ChannelResponse(
	string Id,
	string Name,
	string Topic,
	DateTime CreateAt,
	string? Category,
	string? EventId
)
{
	public static ChannelResponse FromEntity(Channel c) =>
		new(c.Id, c.Name, c.Topic, c.CreateAt, c.Category, c.EventId);
}

public static partial class ChannelSlug
{
	[GeneratedRegex(@"\s+")]
	private static partial Regex Whitespace();

	[GeneratedRegex(@"[$%^&*()+|~={}\[\]:;<>?,.\/\\`´'""!@#]")]
	private static partial Regex Forbidden();

	[GeneratedRegex(@"-{2,}")]
	private static partial Regex DashRuns();

	public static string Sanitize(string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return string.Empty;

		var s = input.ToLowerInvariant();
		s = Whitespace().Replace(s, "-");
		s = Forbidden().Replace(s, "");
		s = DashRuns().Replace(s, "-");
		return s.Trim('-');
	}
}

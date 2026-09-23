using System.Text.RegularExpressions;
using KeepGrouped.API.Events;
using KeepGrouped.API.Roles;

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
	public string Name { get; set; } = null!;
	public string Topic { get; set; } = null!;
	public uint Order { get; init; } = 0;
	public DateTime CreateAt { get; } = DateTime.UtcNow;
	public string? Category { get; set; } = null;
	public string? EventId { get; set; } = null;
	public Event? Event { get; set; } = null;

	public ICollection<ChannelRole> RolesWhitelist { get; set; } = [];
	public bool CategorySync { get; set; } = true;
}

public record ChannelResponse(
	string Id,
	string Name,
	string Topic,
	DateTime CreateAt,
	string? Category,
	string? EventId,
	IReadOnlyList<ChannelRoleResponse> RolesWhitelist,
	bool CategorySync
)
{
	public static ChannelResponse FromEntity(Channel c) =>
		new(
				c.Id,
				c.Name,
				c.Topic,
				c.CreateAt,
				c.Category,
				c.EventId,
				c.RolesWhitelist.Select((r) => new ChannelRoleResponse(r.Role.Id, r.Role.Name)).ToList(),
				c.CategorySync
		);
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

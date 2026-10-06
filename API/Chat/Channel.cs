using System.Text.RegularExpressions;
using KeepGrouped.API.Events;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Chat;

public class Channel : IRoleWhitelist
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
	public string? CategoryId { get; set; } = null;
	public ChannelCategory? Category { get; set; } = null;
	public string? EventId { get; set; } = null;
	public Event? Event { get; set; } = null;

	public ICollection<ChannelRole> RolesWhitelist { get; set; } = [];
	public bool CategorySync { get; set; } = true;

	public bool IsWhitelisted(User sender)
	{
		if (sender.Role.Permission.HasFlag(Perms.HandleChannels))
			return true;

		if (CategorySync && Category is not null)
		{
			if (Category.RolesWhitelist.Count > 0)
			{
				var isWhitelisted = Category.RolesWhitelist.Any(r => r.Role.Id == sender?.Role.Id);
				if (!isWhitelisted)
					return false;
			}
			return true;
		}

		if (RolesWhitelist.Count > 0)
		{
			var isWhitelisted = RolesWhitelist.Any(r => r.Role.Id == sender?.Role.Id);
			if (!isWhitelisted)
				return false;
		}
		return true;
	}
}

public record ChannelResponse(
	string Id,
	string Name,
	string Topic,
	DateTime CreateAt,
	string? CategoryId,
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
				c.CategoryId,
				c.EventId,
				[.. c.RolesWhitelist.Select((r) => new ChannelRoleResponse(r.Role.Id, r.Role.Name))],
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

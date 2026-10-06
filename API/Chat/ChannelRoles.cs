using KeepGrouped.API.Problems;
using KeepGrouped.API.Roles;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public interface IRoleWhitelist
{
	string Id { get; }
	ICollection<ChannelRole> RolesWhitelist { get; }
}

public class ChannelRole
{
	public string Id { get; init; } = Guid.NewGuid().ToString();
	public string? ChannelId { get; init; } = null;
	public Channel? Channel { get; init; } = null;
	public string? CategoryId { get; init; } = null;
	public ChannelCategory? Category { get; init; } = null;
	public string RoleId { get; init; } = null!;
	public Role Role { get; init; } = null!;

	public static async Task<Result<bool>> UpdateRoles(
		KeepGroupedDb db,
		List<string> newRoles,
		IRoleWhitelist entity,
		Func<IRoleWhitelist, Role, ChannelRole> createRole)
	{
		var roles = await db.Roles
			.ToDictionaryAsync(r => r.Name);

		if (newRoles.Any(r => !roles.ContainsKey(r)))
			return ChannelProblems.RoleNotFound();

		var newRoleIds = newRoles
			.Select(r => roles[r])
			.ToHashSet();

		var existing = entity.RolesWhitelist.ToList();

		var toRemove = existing
			.Where(x => !newRoleIds.Any(role => x.RoleId == role.Id))
			.ToList();

		var toAdd = newRoleIds
			.Where(role => !existing.Any(x => x.RoleId == role.Id))
			.Select(role => createRole(entity, role))
			.ToList();

		db.ChannelRoles.RemoveRange(toRemove);
		db.ChannelRoles.AddRange(toAdd);

		return true;
	}
}

public record ChannelRoleResponse(string Id, string Name)
{
	public static ChannelRoleResponse FromEntity(string id, string name) => new(id, name);
}

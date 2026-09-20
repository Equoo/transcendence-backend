using KeepGrouped.API.Users;

namespace KeepGrouped.API.Roles;

public enum Perms
{

	// Event
	HandleEvent = 2 ^ 1,

	// User
	HandleUsers = 2 ^ 2,
	InviteUser = 2 ^ 3,

	// Chat
	HandleChannels = 2 ^ 4,
	ManageMessages = 2 ^ 5,

	// Roles
	HandleRoles = 2 ^ 6,

	// Knowledge
	HandleKnowledge = 2 ^ 7,
}

public class Role
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string Name { get; set; } = null!;
	public int Permission { get; set; } = 0;

	public ICollection<User> Users { get; } = new List<User>();

	public Role(string name)
	{
		Name = name;
	}

	public Role(string name, int perm)
	{
		Name = name;
		Permission = perm;
	}
}

using KeepGrouped.API.Users;

namespace KeepGrouped.API.Roles;

[Flags]
public enum Perms
{
    // Event
    HandleEvent = 1 << 0,

    // User
    HandleUsers = 1 << 1,
    InviteUser = 1 << 2,

    // Chat
    HandleChannels = 1 << 3,
    ManageMessages = 1 << 4,

    // Roles
    HandleRoles = 1 << 5,

    // Knowledge
    HandleKnowledge = 1 << 6,
}

public class Role
{
    public const string SuperRole = "\\(*-*)/";

    public const string DefaultRole = "Member";

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public int Ranking { get; set; }
    public Perms Permission { get; set; } = 0;

    public ICollection<User> Users { get; } = new List<User>();

    public Role(string name, int ranking)
    {
        Name = name;
        Ranking = ranking;
    }

    public Role(string name, Perms perm, int ranking)
    {
        Name = name;
        Permission = perm;
        Ranking = ranking;
    }
}

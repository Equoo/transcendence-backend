using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Activities;

namespace KeepGrouped.API.Users;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Role Role { get; set; } = null!;

    public StorageFile? Avatar { get; set; }
    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public ICollection<ChannelAck> ChannelsAckMsg { get; } = [];
    public bool IsOnline { get; set; } = false;
}

/// <summary>
/// The public face of a user, embedded wherever another resource points at one (an event organizer,
/// a registration, a file creator). Shared on purpose: it is one projection, not one per use case.
/// </summary>
public record UserSummary(string Id, string UserName, RoleResponse Role)
{
    public static UserSummary FromEntity(User user) => new(user.Id, user.UserName, RoleResponse.FromEntity(user.Role));
}

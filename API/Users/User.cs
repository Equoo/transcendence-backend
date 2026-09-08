using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;

namespace KeepGrouped.API.Users;

public enum Activity
{
    Online,
    Afk,
    Busy,
    Invisible,
    Offline,
}

public class User
{
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public User() { }

    public User(string username)
    {
        UserName = username;
    }

    public User(string username, string id)
    {
        UserName = username;
        Id = id;
    }

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public Activity Activity { get; set; } = Activity.Offline;

    public ICollection<ChannelAck> ChannelsAckMsg { get; } = [];
    public bool IsOnline { get; set; } = false;
}

/// <summary>
/// The public face of a user, embedded wherever another resource points at one (an event organizer,
/// a registration, a file creator). Shared on purpose: it is one projection, not one per use case.
/// </summary>
public record UserSummary(string Id, string UserName, Activity Activity)
{
    public static UserSummary FromEntity(User user) => new(user.Id, user.UserName, user.Activity);
}

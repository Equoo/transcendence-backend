using KeepGrouped.API.Channels;
using KeepGrouped.API.Events;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Activities;
using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.Users;

public enum LanguageEnum
{
    Francais,
    English,
    汉语,
}

public class User : IExpirable
{
    public DateTime ExpireAt { get; set; }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = null!;
    [Key]
    [Length(1, 255)]
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public LanguageEnum Language { get; set; } = LanguageEnum.English;
    public Role Role { get; set; } = null!;
    public StorageFile? Avatar { get; set; }
    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public ICollection<ChannelAck> ChannelsAckMsg { get; } = [];
    public bool IsOnline { get; set; } = false;

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}

public record UserSummary(string Id, string UserName, RoleResponse Role)
{
    public static UserSummary FromEntity(User user) => new(user.Id, user.UserName, RoleResponse.FromEntity(user.Role));
}

using KeepGrouped.API.Users;

namespace KeepGrouped.API.Events;

public class Registration
{
    public User User { get; set; } = null!;

    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public EventRole Role { get; set; } = null!;
}

/// <summary>A registration as embedded in an event response.</summary>
public record RegistrationSummary(UserSummary User, DateTime RegisteredAt, string? Role)
{
    public static RegistrationSummary FromEntity(Registration reg) => new(
        UserSummary.FromEntity(reg.User),
        reg.RegisteredAt,
        reg.Role.Name);
}

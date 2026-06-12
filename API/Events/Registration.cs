using KeepGrouped.API.Users;

namespace KeepGrouped.API.Events;

public class Registration
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string EventId { get; set; } = string.Empty;
    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }
    public RegistrationRole Role { get; set; }

}

public enum RegistrationRole
{
    Participant,
    Organizer,
}
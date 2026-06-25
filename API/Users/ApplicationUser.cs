using KeepGrouped.API.Events;
using Microsoft.AspNetCore.Identity;

namespace KeepGrouped.API.Users;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser() : base() { }
    public ApplicationUser(string username) : base(username) { }

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}

public record UserResponse(string Id, string UserName)
{
    public static UserResponse FromEntity(ApplicationUser user) => new(user.Id, user.UserName!);
}
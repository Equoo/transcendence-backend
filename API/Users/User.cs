using KeepGrouped.API.Events;

namespace KeepGrouped.API.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class User
{
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Role? Role { get; set; } = null;

    public User() { }

    public User(string username)
    {
        UserName = username;
    }


	public User(string username, Role role)
	{
		UserName = username;
		Role = role;
	}

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}

/// <summary>
/// The public face of a user, embedded wherever another resource points at one (an event organizer,
/// a registration, a file creator). Shared on purpose: it is one projection, not one per use case.
/// </summary>
public record UserSummary(string Id, string UserName)
{
    public static UserSummary FromEntity(User user) => new(user.Id, user.UserName);
}

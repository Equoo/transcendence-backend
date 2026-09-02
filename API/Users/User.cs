using KeepGrouped.API.Events;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.Users;



public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Role Role { get; set; } = null!;

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

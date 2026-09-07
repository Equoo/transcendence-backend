using System.ComponentModel.DataAnnotations;
using Amazon.Util.Internal;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public enum Perms
{
    isAdmin = 1,

    // Event
    HandleEvent = 2,

    // User
    HandleUsers = 4,
    InviteUser = 8,

    // Chat
    HandleChannels = 16,

    // Roles
    HandleRoles = 32,

    // Knowledge
    HandleKnowledge = 64,
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

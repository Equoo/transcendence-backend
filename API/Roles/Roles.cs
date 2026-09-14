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

    // Event
    HandleEvent = 1,

    // User
    HandleUsers = 2,
    InviteUser = 4,

    // Chat
    HandleChannels = 8,

    // Roles
    HandleRoles = 16,

    // Knowledge
    HandleKnowledge = 32,
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

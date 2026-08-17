using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c

namespace KeepGrouped.API.Users;

public enum Activity
<<<<<<< HEAD
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

	public Dictionary<string, string> ChannelsAckMsg = [];
	public bool IsOnline { get; set; } = false;
}

/// <summary>
/// The public face of a user, embedded wherever another resource points at one (an event organizer,
/// a registration, a file creator). Shared on purpose: it is one projection, not one per use case.
/// </summary>
public record UserSummary(string Id, string UserName, Activity Activity)
{
	public static UserSummary FromEntity(User user) => new(user.Id, user.UserName, user.Activity);
=======
{
    Online,
    Afk,
    Busy,
    Invisible,
    Offline,
}

public class User : IdentityUser
{
    public User()
        : base() { }

    public User(string username)
        : base(username) { }

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public Activity Activity { get; set; } = Activity.Offline;

    public Dictionary<string, string> ChannelsAckMsg = [];
    public bool IsOnline { get; set; } = false;
}

public record UserRequest
{
    public string UserName { get; init; } = null!;
    public Activity Activity { get; init; } = Activity.Offline;
    public string PasswordHash { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null;

    public Dictionary<string, string> ChannelsAckMsg = null!;
    public Dictionary<string, ChannelSetting> ChannelsSettings = null!;
}

public record UserResponse(
    string Id,
    string UserName,
    string Email,
    string? PhoneNumber,
    Dictionary<string, string> ChannelsAckMsg
)
{
    public static UserResponse FromEntity(User usr) =>
        new(
            usr.Id,
            usr.UserName ?? "Unknown",
            usr.Email ?? "Unknown",
            usr.PhoneNumber,
            usr.ChannelsAckMsg
        );
}

public static class UserEndpoint
{
    public static void MapUsers(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users");

        users.MapGet(
            "/",
            async (KeepGroupedDb db) =>
            {
                var user = await db.Users.ToListAsync();

                return Results.Ok(user.Select(x => UserResponse.FromEntity(x)));
            }
        );

        users.MapGet(
            "/{id}",
            async (string id, KeepGroupedDb db) =>
            {
                var user = await db.Users.Where(user => user.Id == id).FirstOrDefaultAsync();

                return user is null
                    ? Results.NotFound()
                    : Results.Ok(UserResponse.FromEntity(user));
            }
        );

        users.MapPost(
            "/register",
            async (KeepGroupedDb db, UserRequest req) =>
            {
                var user = new User
                {
                    UserName = req.UserName,
                    PasswordHash = req.PasswordHash,
                    Email = req.Email,
                    PhoneNumber = req.PhoneNumber,
                };

                // Check info not already used in Db

                var tmp = await db
                    .Users.Where(e =>
                        e.UserName == req.UserName
                        || e.Email == req.Email
                        || e.PhoneNumber == req.PhoneNumber
                    )
                    .ToListAsync();

                if (tmp.Count > 0)
                {
                    int flags = 0;
                    foreach (var user_find in tmp)
                    {
                        if (user_find.UserName == req.UserName)
                            flags |= 1;
                        if (user.Email == req.Email)
                            flags |= 2;
                    }
                    return Results.BadRequest($"Flags information used:{flags}");
                }

                // Hashed password

                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Results.Ok(user);
            }
        );

        // users.MapPost("/login" () => {});

        users.MapGet("/ping", () => "ping TestEndpoint");
    }
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c
}

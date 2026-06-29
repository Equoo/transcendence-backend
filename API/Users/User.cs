using KeepGrouped.API.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace KeepGrouped.API.Users;

public class User : IdentityUser
{
    public User() : base() { }
    public User(string username) : base(username) { }

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}


public record UserRequest
{
    public string UserName { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }

}

public record UserResponse(

    string Id,
    string UserName,
    string Email,
    string? PhoneNumber
)
{
    public static UserResponse FromEntity (User usr) => new (usr.Id, usr.UserName!, usr.Email!, usr.PhoneNumber);
}


public static class UserEndpoint
{
    public static void MapUsers(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users").WithTags("Users");

        users.MapGet("/", async (KeepGroupedDb db) =>
        {
            var user = await db.Users.ToListAsync();

            return Results.Ok(user.Select(x => UserResponse.FromEntity(x)));
        })
        .WithName("users.list")
        .WithSummary("List users")
        .WithDescription("Returns the public profile of every registered user.")
        .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK);

        users.MapGet("/{id}", async (string id, KeepGroupedDb db) =>
        {
            var user = await db
            .Users
            .Where(user => user.Id == id)
            .FirstOrDefaultAsync();

            return user is null ? Results.NotFound() : Results.Ok(UserResponse.FromEntity(user));

        })
        .WithName("users.get")
        .WithSummary("Get a user")
        .WithDescription("Returns the public profile of a single user identified by their id.")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        users.MapPost("/register", async (KeepGroupedDb db, UserRequest req) =>
        {
            var user = new User
            {
                UserName = req.UserName,
                PasswordHash = req.PasswordHash,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber
            };


            // Check info not already used in Db

            var tmp = await db.Users.Where(e => e.UserName == req.UserName || e.Email == req.Email || e.PhoneNumber == req.PhoneNumber).ToListAsync();

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
            return Results.Ok(UserResponse.FromEntity(user));
        })
        .WithName("users.register")
        .WithSummary("Register a user")
        .WithDescription("Creates a user account. The username, email and phone number must not already be in use.")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status400BadRequest);

        // users.MapPost("/login" () => {});

        users.MapGet("/ping", () => "ping TestEndpoint")
        .WithName("users.ping")
        .WithSummary("Ping the users endpoints")
        .WithDescription("Development helper that returns a constant string to check the API is reachable.")
        .Produces<string>(StatusCodes.Status200OK);
    }
}

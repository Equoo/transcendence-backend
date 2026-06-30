using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Events;
using KeepGrouped.API.Password;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace KeepGrouped.API.Users;

public class User : IdentityUser
{
    public User() : base() { }
    public User(string username) : base(username) { }
    
    public string? Saltz { get; set; }

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}


public record UserRequest
{
    [Required]
    public string UserName { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;

}

public record UserResponse(

    string Id,
    string UserName
)
{
    public static UserResponse FromEntity (User usr) => new (usr.Id, usr.UserName!);
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

        users.MapPost("/register", async (KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass, IPasswordValidator<User> test, UserManager<User> manager, IUserValidator<User> testout, SignInManager<User> t) =>
        {

            var user = new User
            {
                UserName = req.UserName,
            };

            // Check info not already used in Db

            var tmp = await db.Users.Where(e => e.UserName == req.UserName ).AnyAsync();
            if (tmp)
                return  Results.BadRequest("UserName already used");


            // Check UserName and password size etc...

            if (!(await test.ValidateAsync(manager, user, req.Password)).Succeeded)
                return Results.BadRequest("Password invalid");

            // Hashed password
            user.PasswordHash = pass.HashPassword(user, req.Password);


            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(UserResponse.FromEntity(user));
        })
        .WithName("users.register")
        .WithSummary("Register a user")
        .WithDescription("Creates a user account. The username, email and phone number must not already be in use.")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status400BadRequest);

        users.MapPost("/login", async (KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass) =>
        {
            User? user_db = await db.Users.SingleOrDefaultAsync(u => u.UserName == req.UserName);

            if (user_db is null)
            {
                return Results.BadRequest("Probleme during connexion");
            }

            if (pass.VerifyHashedPassword(user_db, user_db.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
            {
                return Results.BadRequest("Bad password authentification");
            }

            return Results.Ok($"connected to {req.UserName}");
        });

        users.MapGet("/ping", () => "ping TestEndpoint")
        .WithName("users.ping")
        .WithSummary("Ping the users endpoints")
        .WithDescription("Development helper that returns a constant string to check the API is reachable.")
        .Produces<string>(StatusCodes.Status200OK);
    }
}

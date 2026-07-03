using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KeepGrouped.API.Events;
using Microsoft.AspNetCore.Authorization;
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
    [Required]
    public string UserName { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;

}

public class UserResponse
{
    public string? Id { get; set; } = null;
    public string? UserName { get; set; } = null;

    public UserResponse(){}

    public UserResponse(string id, string username)
    {
        Id = id;
        UserName = username;
    }
        
    public static UserResponse FromEntity (User usr) => new (usr.Id, usr.UserName!);
}

public static class UserEndpoint
{    
    public static void MapUsers(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users").WithTags("Users");

        // -------------- Return all users

        users.MapGet("/", [Authorize] async (KeepGroupedDb db) =>
        {
            var user = await db
            .Users
            .ToListAsync();

            return Results.Ok(user.Select(x => UserResponse.FromEntity(x)));
        })
        .WithName("users.list")
        .WithSummary("List users")
        .WithDescription("Returns the public profile of every registered user.")
        .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK);

         // -------------- Return user from id

        users.MapGet("/{id}", [Authorize] async (string id, KeepGroupedDb db) =>
        {
            var user = await db
            .Users
            .FirstOrDefaultAsync(user => user.Id == id);

            return user is null ? Results.NotFound() : Results.Ok(UserResponse.FromEntity(user));

        })
        .WithName("users.get")
        .WithSummary("Get a user")
        .WithDescription("Returns the public profile of a single user identified by their id.")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

         // -------------- Create user 

        users.MapPost("/register", async (IHostEnvironment env, KeepGroupedDb db, UserRequest req, IPasswordHasher<User> hash, IPasswordValidator<User> pass, UserManager<User> manager) =>
        {

            var user = new User
            {               
                Id = Guid.NewGuid().ToString().GetHashCode().ToString("x"),
                UserName = req.UserName
            };
            
            // Check password resistance
            if (!(await pass.ValidateAsync(manager, user, req.Password)).Succeeded)
                return Results.BadRequest("Password invalid");

            // Check duplicate
            var dup_usr = await db
            .Users
            .AnyAsync(e => e.UserName == req.UserName);
            
            if (dup_usr)
                return  Results.BadRequest("UserName already used");

            // Hashed password
            user.PasswordHash = hash.HashPassword(user, req.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(UserResponse.FromEntity(user));
        })
        .WithName("users.register")
        .WithSummary("Register a user")
        .WithDescription("Creates a user account. The username, email and phone number must not already be in use.")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status400BadRequest);

         // -------------- Authenticate user and adding JWT

        users.MapPost("/login", async (KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass, HttpContext context) =>
        {
            User? user_db = await db.Users.SingleOrDefaultAsync(u => u.UserName == req.UserName);

            if (user_db is null)
                return Results.BadRequest("Probleme during connexion");

            if (pass.VerifyHashedPassword(user_db, user_db.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
                return Results.BadRequest("Bad password authentification");

            // Handle Json Web Token
            var token = Token.Build(user_db.UserName, user_db.Id);
            Token.AddToCookie(token, context);

            return Results.Ok("You are now connected !");
        });

         // -------------- Remove JWT

        users.MapGet("/logout", [Authorize] async (HttpContext context) =>
        {
            context.Response.Cookies.Delete("AuthToken");
            return Results.Ok("You are now logout !");
        });

         // -------------- Return connected User

        users.MapGet("/me", [Authorize] (HttpContext context) =>
        {
            var cookie = context.Request.Cookies["AuthToken"];

            UserResponse resp = new();            
            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
            
            foreach(Claim claim in token.Claims)
            {
                if (claim.Type == ClaimTypes.Name)
                    resp.UserName = claim.Value;
                if (claim.Type == ClaimTypes.NameIdentifier)
                    resp.Id = claim.Value;
            }

            return Results.Ok(resp);
        });

    }
}
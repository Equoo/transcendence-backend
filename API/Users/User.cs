using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using KeepGrouped.API.Events;
using KeepGrouped.API.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using KeepGrouped.API;
using Npgsql.Replication;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Routing;

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

    public static string BuildToken(string name)
    {
         var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "admin")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: "KeepGrouped",
                audience: "KeepGrouped",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

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

        users.MapPost("/register", async (IHostEnvironment env, KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass, IPasswordValidator<User> test, UserManager<User> manager, IUserValidator<User> testout, SignInManager<User> t) =>
        {

            var user = new User
            {
                UserName = req.UserName,
            };

            

            if (!HostEnvironmentEnvExtensions.IsDevelopment(env))
            {     
                if (!(await test.ValidateAsync(manager, user, req.Password)).Succeeded)
                    return Results.BadRequest("Password invalid");
            }
            // Check info not already used in Db

            var tmp = await db.Users.Where(e => e.UserName == req.UserName ).AnyAsync();
            if (tmp)
                return  Results.BadRequest("UserName already used");


            // Check UserName and password size etc...

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

        users.MapPost("/login", async (KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass, HttpContext context) =>
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

            context.Response.Cookies.Append("Token", BuildToken(req.UserName), new CookieOptions
            {
                HttpOnly = true,
                Secure = false
            });
            // Creation of JWT 
            return Results.Ok("Cookie sent");


        });

        users.MapGet("/ping" , [Authorize] () => "ping TestEndpoint")
        .WithName("users.ping")
        .WithSummary("Ping the users endpoints")
        .WithDescription("Development helper that returns a constant string to check the API is reachable.")
        .Produces<string>(StatusCodes.Status200OK);
    }
}
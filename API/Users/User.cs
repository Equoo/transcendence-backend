using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KeepGrouped.API.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

public class User : IdentityUser
{
    public User() : base() { }
    public User(string username) : base(username) { }

    public User Clone()
    {
        return new User{Id = this.Id, UserName = this.UserName};
    }

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

        users.MapGet("/", async (KeepGroupedDb db) =>
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

         
        var me = app.MapGroup("/me");

        // -------------- Return connected User

        me.MapGet("/", [Authorize] (TokenContext tk) =>
        {
            return Results.Ok(UserResponse.FromEntity(tk.User));
        });

        // -------------- Change UserName

        me.MapPut("/", [Authorize] async (UserRequest req, KeepGroupedDb db, TokenContext tk) =>
        {
            tk.User.UserName = req.UserName;

            await db.SaveChangesAsync();

            return Results.Ok(UserResponse.FromEntity(tk.User));
        });


        // -------------- Delete User

        me.MapDelete("/", [Authorize] async (KeepGroupedDb db, HttpContext http, TokenContext tk) =>
        {
            db.Users.Remove(tk.User);
            await db.SaveChangesAsync();

            Token.RemoveTokenCookie(http);

            return Results.Ok(UserResponse.FromEntity(tk.User));
        });
    }
}
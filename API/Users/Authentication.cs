using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Problems;

namespace KeepGrouped.API.Users;

public static class AuthenticationEndpoint
{
    public static void MapAuthentication(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/auth");

         // -------------- Create user 

        auth.MapPost("/register", async (IHostEnvironment env, KeepGroupedDb db, UserRequest req, IPasswordHasher<User> hash, HttpContext http) =>
        {

            var user = new User
            {               
                
                UserName = req.UserName
            };
            
            // Check password resistance
            

            // Check duplicate
            var dup_usr = await db
            .Users
            .AnyAsync(e => e.UserName == req.UserName);

            if (dup_usr)
                return  Problems.UserProblems.NameAlreadyUsed(req.UserName);

            // Hashed password
            user.PasswordHash = hash.HashPassword(user, req.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = Token.Build(user.Id);
            Token.AddTokenCookie(token, http);
            
            return Results.Created("/users/{id}", UserResponse.FromEntity(user));
        });


        // -------------- Authenticate user and adding JWT

        auth.MapPost("/login", async (KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass, HttpContext context) =>
        {
            User? user_db = await db.Users.SingleOrDefaultAsync(u => u.UserName == req.UserName);

            if (user_db is null)
                return Problems.UserProblems.AuthenticationInvalid();

            if (pass.VerifyHashedPassword(user_db, user_db.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
                return Problems.UserProblems.AuthenticationInvalid();

            // Handle Json Web Token
            var token = Token.Build(user_db.Id);
            Token.AddTokenCookie(token, context);

            return Results.Ok();
        });

        // -------------- Remove JWT

        auth.MapPost("/logout", [Authorize] async (HttpContext http) =>
        {
            Token.RemoveTokenCookie(http);
            return Results.Ok();
        });

        auth.MapPost("/refresh", [Authorize] () => "Refresh the token !");
    }

}
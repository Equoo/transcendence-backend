using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Server.HttpSys;

namespace KeepGrouped.API.Tests;


public class Test : IdentityUser {}


public record TestRequest
{
    [Required]
    public string UserName { get; init; } = null!;
    [Required]
    public string PasswordHash { get; init; } = null!;
    [Required]
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null;

}

public record TestResponse(

    string Id,
    string UserName,
    string Email,
    string? PhoneNumber
)
{
    public static TestResponse FromEntity (Test te) => new (te.Id, te.UserName, te.Email, te.PhoneNumber);
}


public static class TestEndpoint
{
    public static void Map(WebApplication app)
    {
        var users = app.MapGroup("/users");

        users.MapGet("/", async (KeepGroupedDb db) =>
        {
            var user = await db.Test.ToListAsync();
            
            return Results.Ok(user.Select(x => TestResponse.FromEntity(x)));
        });

        users.MapGet("/{id}", async (string id, KeepGroupedDb db) =>
        {
            var user = await db
            .Test
            .Where(user => user.Id == id)
            .FirstOrDefaultAsync();

            return user is null ? Results.NotFound() : Results.Ok(TestResponse.FromEntity(user));
        });

        users.MapPost("/register", async (KeepGroupedDb db, TestRequest req) =>
        {
            var user = new Test
            {
                UserName = req.UserName,
                PasswordHash = req.PasswordHash,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber
            };

            // Check info not already used in Db

            var tmp = await db.Test.Where(e => e.UserName == req.UserName || e.Email == req.Email || e.PhoneNumber == req.PhoneNumber).ToListAsync(); 
            
            if (tmp.Count > 0)
            {
                int flags = 0;
                foreach(var user_find in tmp)
                {
                    if (user_find.UserName == req.UserName)
                        flags |= 1;
                    if (user.Email == req.Email)
                        flags |= 2;
                    if (user_find.PhoneNumber == req.PhoneNumber)
                        flags |= 4;
                }
               return Results.BadRequest($"Flags information used:{flags}");
            }


            db.Test.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(user);
        });

        users.MapGet("/ping", () => "ping TestEndpoint");
    }
}


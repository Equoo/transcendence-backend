using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace KeepGrouped.API.Tests;


public class Test : IdentityUser {}


public record TestRequest
{
    [Required]
    public string? UserName { get; init; }
    [Required]
    public string? PasswordHash { get; init; }
    [Required]
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; } = null;

}

public record TestResponse(

    string Id,
    string UserName,
    string Email,
    string PhoneNumber
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
            var user = await db.Test.SingleAsync();
            
            var response = TestResponse.FromEntity(user);
            return Results.Ok(response);
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
            db.Test.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(user);
        });

        users.MapGet("/ping", () => "ping TestEndpoint");
    }
}


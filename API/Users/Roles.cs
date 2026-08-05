using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput;

namespace KeepGrouped.API.Users;

public enum Perms
{
    isAdmin = 1,
    
    // Event
    CreateEvent = 2,
    DeleteEvent = 4,
    GetEvents = 8,
    ChangeEvent = 16,
    
    // User
    GetUser = 32,
    CreateUser = 64,
    ChangeUserName = 128,
    DeleteUser = 256,
    ResetUserPassword = 516,

    // Chat
    SendMessage = 1024,
    CreateChannel = 2048,
    DeleteChannel = 5096,

    // Knowledge

    // Calendar
}

public class Role
{
    public string Id {get; set; } = Guid.NewGuid().ToString();
    public string Name {get; set;} = null!;

    public int Permission {get; set;} = 0;
}

public record RoleRequest
{
    [Required]
    public string Name {get; init;} = null!;
    [Required]
    public int Permission {get; set;} = 0;
}


public static class RolesEndpoints
{

    public static void MapRoles(this IEndpointRouteBuilder app)
    {

        var role = app.MapGroup("/roles").WithTags("Roles");

        // -------------- Create a new roles
        role.MapPost("/", [Authorize] async (KeepGroupedDb db, RoleRequest request) =>
        {
            Role role = new()
            {
              Name = request.Name,
              Permission = request.Permission
            };



            db.Roles.Add(role);
            await db.SaveChangesAsync();
            return Results.Ok(role);
        });

        // -------------- Adding role to my user
        role.MapPost("/add/${name}", async (string name, TokenContext tk, KeepGroupedDb db) =>
        {
           Role? db_role = await db.Roles.SingleOrDefaultAsync(o => o.Name == name);
           if (db_role is null)
        {
                return Results.BadRequest();
            } 
            tk.User.Role = db_role;
            await db.SaveChangesAsync();
            return Results.Ok(db_role);
        });

        // -------------- Return user roles

        role.MapGet("/", [Authorize] async (TokenContext tk) =>
        {
            return Results.Ok(tk.User.Role);
        });

        role.MapGet("/kg-admin", [Authorize] [Roles ((int) Perms.isAdmin)]  async () => "Administator panel");
    }

}
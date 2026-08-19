using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Roles;

public enum Perms
{
	isAdmin = 1,

	// Event
	HandleEvent = 2,

	// User
	GetUser = 32,
	CreateUser = 64,
	ChangeUserName = 128,
	DeleteUser = 256,
	ResetUserPassword = 516,

	// Chat
	HandleChannel = 2048,

	// Knowledge

	// Calendar
}

public class Role
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string Name { get; set; } = null!;
	public int Permission { get; set; } = 0;
}




	// 	var role = app.MapGroup("/roles").WithTags("Roles");

	// 	// -------------- Return all Roles

	// 	role.MapGet("/", [Authorize] async (KeepGroupedDb db) =>
	// 	{
	// 		List<Role>? db_roles = await db.Roles.ToListAsync();
	// 		if (db_roles is null)
	// 		{
	// 			return Results.NoContent();
	// 		}
	// 		return Results.Ok(db_roles);
	// 	});

	// 	// -------------- Create a new roles
	// 	role.MapPost("/", [Authorize] async (KeepGroupedDb db, RoleRequest request) =>
	// 	{
	// 		Role role = new()
	// 		{
	// 			Name = request.Name,
	// 			Permission = request.Permission
	// 		};


	// 		db.Roles.Add(role);
	// 		await db.SaveChangesAsync();
	// 		return Results.Ok(role);
	// 	});

	// 	// -------------- Adding role to my user
	// 	role.MapPost("/add/${name}", async (string name, TokenContext tk, KeepGroupedDb db) =>
	// 	{
	// 		Role? db_role = await db.Roles.SingleOrDefaultAsync(o => o.Name == name);
	// 		if (db_role is null)
	// 		{
	// 			return Results.BadRequest();
	// 		}
	// 		tk.User.Role = db_role;
	// 		await db.SaveChangesAsync();
	// 		return Results.Ok(db_role);
	// 	});

	// 	// Adding role to a specific user

	// 	role.MapPost("/give/{id_user}/{id_role}", async (string id_user, string id_role, KeepGroupedDb db) =>
	// 	{
	// 		User? db_user = await db.Users.SingleOrDefaultAsync(u => u.Id == id_user);

	// 		if (db_user is null)
	// 		{
	// 			return Results.BadRequest();
	// 		}

	// 		Role? db_role = await db.Roles.SingleOrDefaultAsync(o => o.Id == id_role);

	// 		if (db_role is null)
	// 		{
	// 			return Results.BadRequest();
	// 		}

	// 		db_user.Role = db_role;

	// 		await db.SaveChangesAsync();

	// 		return Results.Ok();
	// 	});



	// 	role.MapGet("/kg-admin", [Authorize][Roles((int)Perms.isAdmin)] async () => "Administator panel");
	// }

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Problems;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.Arm;
using System.Text;


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

			// Check duplicate
			var dup_usr = await db
			.Users
			.AnyAsync(e => e.UserName == req.UserName);

			if (dup_usr)
				return UserProblems.NameAlreadyUsed(req.UserName);

			// Hashed password
			user.PasswordHash = hash.HashPassword(user, req.Password);

			db.Users.Add(user);

			//Acess TOKEN
			string acess_token = Token.BuildAcess(user.Id, http);

			// Refresh TOKEN
			string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
			string refresh_token = Token.BuildRefresh(id, http);

			RefreshToken refresh = new()
			{ 
				Id = id,
				UserId = user.Id,
				ExpireAt = DateTime.Now.AddMinutes(2).Kind
			};

			db.RefreshTokens.Add(refresh);

			await db.SaveChangesAsync();

			return Results.Created("/users/{id}", UserResponse.FromEntity(user));
		});


		// -------------- Authenticate user and adding JWT

		auth.MapPost("/login", async (KeepGroupedDb db, UserRequest req, IPasswordHasher<User> pass, HttpContext http) =>
		{
			User? user_db = await db.Users.SingleOrDefaultAsync(u => u.UserName == req.UserName);

			if (user_db is null)
				return UserProblems.AuthenticationInvalid();

			if (pass.VerifyHashedPassword(user_db, user_db.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
				return UserProblems.AuthenticationInvalid();

			// Acess TOKEN
			var token = Token.BuildAcess(user_db.Id, http);

			// Refresh TOKEN
			string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
			string refresh_token = Token.BuildRefresh(id, http);

			byte[] hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(id));

			RefreshToken refresh = new()
			{
				IdHashed = hash,
				UserId = user_db.Id,
				ExpireAt = DateTime.Now.AddMinutes(2).Kind
			};

			db.RefreshTokens.Add(refresh);

			await db.SaveChangesAsync();

			return Results.Ok();
		});

		// -------------- Remove JWT

		auth.MapPost("/logout", [Authorize] async (HttpContext http) =>
		{
			Token.RemoveCookies(http);
			return Results.Ok();
		});

		auth.MapGet("/refresh", async (HttpContext http, KeepGroupedDb db) =>
		{

			string? cookie_refresh = http.Request.Cookies["RefreshToken"];

			if (cookie_refresh is null || !Token.IsValid(cookie_refresh))
			{
				Token.RemoveCookies(http);
				return Results.BadRequest();
			}

			JwtSecurityToken refresh_token = new JwtSecurityTokenHandler().ReadJwtToken(cookie_refresh);

			// Can have any if your are log in different computer in the same account
			RefreshToken? refresh_db = await db.RefreshTokens.FirstOrDefaultAsync(o => o.Id == refresh_token.Claims.First().Value);

			if (refresh_db is null)
			{
				Token.RemoveCookies(http);
				return Results.BadRequest();
			}

			//Renew AccesToken
			string new_acess = Token.BuildAcess(refresh_db.UserId, http);

			//Renew RefreshToken
			string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
			string new_refresh = Token.BuildRefresh(id, http);


			RefreshToken new_refresh_db = new()
			{
				Id = id,
				UserId = refresh_db.UserId,
				ExpireAt = DateTime.Now.AddMinutes(2).Kind
			};

			db.RefreshTokens.Remove(refresh_db);
			db.RefreshTokens.Add(new_refresh_db);

			await db.SaveChangesAsync();

			return Results.Ok();
		});
	}

}
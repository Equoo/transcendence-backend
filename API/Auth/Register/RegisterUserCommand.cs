using System.Security.Cryptography;
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users.Invitations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

/// <summary>Handler-to-endpoint carrier: the endpoint alone turns the tokens into cookies.</summary>
public sealed record RegisteredUser(RegisterResponse User, IssuedTokens Tokens);

public sealed class RegisterUserCommand(KeepGroupedDb db, KeepGroupedPasswordHasher hash, TokenProvider provider) : IHandler
{
	public async Task<Result<RegisteredUser>> ExecuteAsync(RegisterRequest req)
	{
		var invitation = await db.Invitations.SingleOrDefaultAsync(inv => inv.Id == req.InvitationCode);
		if (invitation is null)
		{
			return InvitationProblems.InvalidInvitation();
		}
		if (invitation.Usages != Invitation.Infinite && ((DateTime.UtcNow >= invitation.ExpireAt) || invitation.Usages == 0))
		{
			db.Invitations.Remove(invitation);
			await db.SaveChangesAsync();
			return InvitationProblems.InvalidInvitation();
		}
		if (await db.Users.AnyAsync(e => e.UserName == req.Username))
		{
			return UserProblems.NameAlreadyUsed(req.Username);
		}
		var email = User.NormalizeEmail(req.Email);
		if (await db.Users.AnyAsync(e => e.Email == email))
		{
			return UserProblems.EmailAlreadyUsed(email);
		}


		var role_db = await db.Roles.SingleAsync(o => o.Name == "Member");


		var user = new User
		{
			ExpireAt = DateTime.UtcNow.AddYears(2),
			UserName = req.Username,
			Email = email,
			Role = role_db
		};


		user.PasswordHash = hash.HashPassword(user, req.Password);

		db.Users.Add(user);

		string acess_token = provider.CreateAccess(user.Id);

		string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
		string refresh_token = provider.CreateRefresh(id);

		db.RefreshTokens.Add(new RefreshToken
		{
			Id = Hash256.GetHashSha256(id),
			UserId = user.Id,
			ExpireAt = DateTime.UtcNow.AddMinutes(2)
		});

		if (invitation.Usages != Invitation.Infinite)
		{
			invitation.Usages -= 1;
		}
		await db.SaveChangesAsync();

		return new RegisteredUser(RegisterResponse.FromEntity(user), new IssuedTokens(acess_token, refresh_token));
	}
}

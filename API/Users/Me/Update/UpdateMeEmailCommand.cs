using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Users;

public sealed class UpdateMeEmailCommand(KeepGroupedDb db, KeepGroupedPasswordHasher pass, IOptions<AuthenticationOptions> option) : IHandler
{
	public async Task<Result<UpdateMeEmailResponse>> ExecuteAsync(User user, UpdateMeEmailRequest req)
	{
		if (option.Value.DefaultAdminLogin == user.UserName)
		{
			return UserProblems.EmailCannotBeChanged();
		}

		if (pass.VerifyHashedPassword(user, user.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
		{
			return MeProblems.InvalidPassword();
		}

		var email = User.NormalizeEmail(req.Email);
		if (await db.Users.AnyAsync(u => u.Email == email && u.Id != user.Id))
		{
			return UserProblems.EmailAlreadyUsed(email);
		}

		user.Email = email;

		try
		{
			await db.SaveChangesAsync();
		}
		catch (DbUpdateException)
		{
			return UserProblems.EmailAlreadyUsed(email);
		}

		return UpdateMeEmailResponse.FromEntity(user);
	}
}

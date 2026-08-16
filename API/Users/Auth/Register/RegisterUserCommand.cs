using System.Security.Cryptography;
using KeepGrouped.API.Password;
using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users;

/// <summary>Handler-to-endpoint carrier: the endpoint alone turns the tokens into cookies.</summary>
public sealed record RegisteredUser(RegisterResponse User, IssuedTokens Tokens);

public sealed class RegisterUserCommand(KeepGroupedDb db, IPasswordHasher<User> hash) : IHandler
{
    public async Task<Result<RegisteredUser>> ExecuteAsync(RegisterRequest req)
    {
        var invitation = await db.Invitations.SingleOrDefaultAsync();
        if (invitation is null)
        {
            return InvitationProblems.InvalidInvitation();
        }
        if ((DateTime.UtcNow > invitation.ExpiresAt) || invitation.Usages < 1)
        {
            db.Invitations.Remove(invitation);
            await db.SaveChangesAsync();
            return InvitationProblems.InvalidInvitation();
        }
        if (await db.Users.AnyAsync(e => e.UserName == req.UserName))
        {
            return UserProblems.NameAlreadyUsed(req.UserName);
        }

        var user = new User
        {
            UserName = req.UserName
        };

        user.PasswordHash = hash.HashPassword(user, req.Password);

        db.Users.Add(user);

        string acess_token = Token.CreateAccess(user.Id);

        string id = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
        string refresh_token = Token.CreateRefresh(id);

        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Hash256.GetHashSha256(id),
            UserId = user.Id,
            ExpireAt = DateTime.Now.AddMinutes(2).Kind
        });

        invitation.Usages -= 1;
        await db.SaveChangesAsync();

        return new RegisteredUser(RegisterResponse.FromEntity(user), new IssuedTokens(acess_token, refresh_token));
    }
}

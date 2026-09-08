using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Middlewares;

public class TokenContext
{
	public User User { get; set; } = null!;
}

public class TokenContextMiddleware
{
	private readonly RequestDelegate _next;

	public TokenContextMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext http, KeepGroupedDb db, TokenContext tcontext)
	{
		string? token_cookie = TokenCookies.Get(http, "AccessToken");

		if (token_cookie is null)
		{
			await _next(http);
			return;
		}

		if (!Token.CanRead(token_cookie))
		{
			TokenCookies.Remove(http);
		}

		string id = Token.ReadFirstClaim(token_cookie);

		User? db_user = await db.Users.Include(u => u.ChannelsAckMsg).SingleOrDefaultAsync(usr => usr.Id == id);

		if (db_user is null)
		{
			await _next(http);
			return;
		}

		tcontext.User = db_user;

		await _next(http);
	}
}

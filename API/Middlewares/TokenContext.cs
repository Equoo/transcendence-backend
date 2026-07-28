using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;
using System.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
		string? token_cookie = http.Request.Cookies["AccessToken"];

		if (token_cookie is null)
		{
			await _next(http);
			return;
		}

		if (!new JwtSecurityTokenHandler().CanReadToken(token_cookie))
		{
			Token.RemoveCookies(http);
		}

		JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(token_cookie);

		string id = token.Claims.First().Value;

		User? db_user = await db.Users.SingleOrDefaultAsync(usr => usr.Id == id);

		if (db_user is null)
		{
			await _next(http);
			return;
		}

		tcontext.User = db_user;

		await _next(http);
	}
}
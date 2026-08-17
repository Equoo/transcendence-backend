<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Middlewares;

public class TokenContext
{
	public User User { get; set; } = null!;
=======
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Middlewares;

// NOTE: #################### PLACEHOLDER ######################

public class TokenContext
{
    public User User { get; set; } = null!;
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c
}

public class TokenContextMiddleware
{
<<<<<<< HEAD
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

		User? db_user = await db.Users.SingleOrDefaultAsync(usr => usr.Id == id);

		if (db_user is null)
		{
			await _next(http);
			return;
		}

		tcontext.User = db_user;

		await _next(http);
	}
=======
    private readonly RequestDelegate _next;

    public TokenContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext http, KeepGroupedDb db, TokenContext tcontext)
    {
        string? cookie = http.Request.Cookies["UserName"];

        if (cookie is null)
        {
            await _next(http);
            return;
        }

        User? db_user = await db.Users.SingleOrDefaultAsync(usr => usr.UserName == cookie);

        if (db_user is null)
        {
            await _next(http);
            return;
        }

        tcontext.User = db_user;

        await _next(http);
    }
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c
}

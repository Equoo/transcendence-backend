using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;
using System.ComponentModel;

namespace KeepGrouped.API.Middlewares;

public class TokenContext
{
    public string Token {get; set;} = null!;

    public string RefreshToken {get; set;} = null!;
    public User User {get; set;} = null!;
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
        string? token_cookie = http.Request.Cookies["AuthToken"];

        if (token_cookie is null)
        {
            await _next(http);
            return;
        }

        tcontext.Token = token_cookie;
        
        string? refresh_cookie = http.Request.Cookies["RefreshToken"];

        if (refresh_cookie is null)
        {
            await _next(http);
            return;
        }

        tcontext.RefreshToken = refresh_cookie;

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
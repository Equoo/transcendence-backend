using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace KeepGrouped.API.Users;

public class TokenContext
{
    public JwtSecurityToken Token {get; set;} = null!;
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

        string? cookie = http.Request.Cookies["AuthToken"];

        if (cookie is null)
        {
            await _next(http);
            return;
        }

        tcontext.Token = new JwtSecurityTokenHandler().ReadJwtToken(cookie);

        string id = tcontext.Token.Claims.First().Value;

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
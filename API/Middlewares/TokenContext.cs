using System;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Middlewares;

// NOTE: #################### PLACEHOLDER ######################

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
}

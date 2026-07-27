using System.IdentityModel.Tokens.Jwt;
using KeepGrouped.API;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TokenContext = KeepGrouped.API.Middlewares.TokenContext;

public class TokenHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public TokenHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TokenContext token, KeepGroupedDb db)
    {
        Console.WriteLine("TOKEN REFRESH IS JUST ON THE NEXT LINEEEE");
        Console.WriteLine(token.RefreshToken);

        await _next(context);
    }
}
using System.Text;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Password;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

static public class AuthenticationBuilder
{
    static public void BuildAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IPasswordHasher<User>, KeepGroupedPasswordHasher>();
        builder.Services.AddScoped<KeepGrouped.API.Middlewares.TokenContext>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = "KeepGrouped",
                ValidAudience = "KeepGrouped",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["AccessToken"];
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Expired", "True");
                        return Task.CompletedTask;
                    }
                    context.Response.Cookies.Delete("AccessToken");
                    return Task.CompletedTask;
                },
            };
        });
    }
}
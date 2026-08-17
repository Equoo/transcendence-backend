using KeepGrouped.API.Middlewares;

namespace KeepGrouped.API.Users;

public static class AuthenticationEndpoint
{
    public static void MapAuthentication(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TokenContextMiddleware>();
        app.UseMiddleware<DelayMiddleware>();

        var auth = app.MapGroup("/auth");

        auth.MapRegister();
        auth.MapLogin();
        auth.MapLogout();
        auth.MapRefresh();
    }
}

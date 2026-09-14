using KeepGrouped.API.Events;
using KeepGrouped.API.Middlewares;

namespace KeepGrouped.API.Users;

public static class AuthenticationEndpoint
{
    public static void MapAuthentication(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TokenContextMiddleware>();

        var auth = app.MapGroup("/auth");

        auth.MapRegister();
        auth.MapLogin();
        auth.MapLogout();
        auth.MapLogoutUser();
        auth.MapResetPassword();
        auth.MapRefresh();
    }
}

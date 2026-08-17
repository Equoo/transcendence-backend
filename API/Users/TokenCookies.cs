namespace KeepGrouped.API.Users;

/// <summary>The only place where authentication tokens meet HTTP. Used from endpoints and middlewares, never from handlers.</summary>
public static class TokenCookies
{
    public static void Write(HttpContext http, IssuedTokens tokens)
    {
        http.Response.Cookies.Append("AccessToken", tokens.AccessToken);
        http.Response.Cookies.Append("RefreshToken", tokens.RefreshToken);
    }

    public static void Remove(HttpContext http)
    {
        http.Response.Cookies.Delete("AccessToken");
        http.Response.Cookies.Delete("RefreshToken");
    }

    public static string? Get(HttpContext http, string key) => http.Request.Cookies[key];
}
